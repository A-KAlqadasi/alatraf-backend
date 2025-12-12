using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.Patients.Dtos;
using AlatrafClinic.Application.Features.Patients.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Patients.Queries.GetPatients;

public class GetPatientsQueryHandler
    : IRequestHandler<GetPatientsQuery, Result<PaginatedList<PatientDto>>>
{
    private readonly IAppDbContext _context;

    public GetPatientsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<PatientDto>>> Handle(
        GetPatientsQuery query,
        CancellationToken ct)
    {
        
        IQueryable<Patient> patientsQuery = _context.Patients
            .Include(p => p.Person)
            .AsNoTracking();

        patientsQuery = ApplyFilters(patientsQuery, query);
        patientsQuery = ApplySearch(patientsQuery, query);
        patientsQuery = ApplySorting(patientsQuery, query);

        var totalCount = await patientsQuery.CountAsync(ct);

        if (totalCount == 0)
        {
            return Error.NotFound(
                "Patients.NotFound",
                "No patients found matching the given filters.");
        }

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 10 : query.PageSize;

        var skip = (page - 1) * pageSize;

        var patients = await patientsQuery
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        var items = patients
            .Select(p => p.ToDto())
            .ToList();

        var result = new PaginatedList<PatientDto>
        {
            Items      = items,
            PageNumber = page,
            PageSize   = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return result;
    }

    // ---------------- FILTERS ----------------
    private static IQueryable<Patient> ApplyFilters(
        IQueryable<Patient> query,
        GetPatientsQuery q)
    {
        if (q.PatientType.HasValue)
        {
            var type = q.PatientType.Value;
            query = query.Where(p => p.PatientType == type);
        }

        if (q.Gender.HasValue)
        {
            var gender = q.Gender.Value;
            query = query.Where(p => p.Person != null && p.Person.Gender == gender);
        }

        if (q.BirthdateFrom.HasValue)
        {
            var from = q.BirthdateFrom.Value;
            query = query.Where(p => p.Person != null && p.Person.Birthdate >= from);
        }

        if (q.BirthdateTo.HasValue)
        {
            var to = q.BirthdateTo.Value;
            query = query.Where(p => p.Person != null && p.Person.Birthdate <= to);
        }

        if (q.HasNationalNo.HasValue)
        {
            if (q.HasNationalNo.Value)
            {
                query = query.Where(p =>
                    p.Person != null &&
                    !string.IsNullOrEmpty(p.Person.NationalNo));
            }
            else
            {
                query = query.Where(p =>
                    p.Person != null &&
                    string.IsNullOrEmpty(p.Person.NationalNo));
            }
        }

        return query;
    }

    // ---------------- SEARCH ----------------
    private static IQueryable<Patient> ApplySearch(
        IQueryable<Patient> query,
        GetPatientsQuery q)
    {
        if (string.IsNullOrWhiteSpace(q.SearchTerm))
            return query;

        var pattern = $"%{q.SearchTerm.Trim().ToLower()}%";

        return query.Where(p =>
            EF.Functions.Like(p.Person!.AutoRegistrationNumber!.ToLower(), pattern) ||
            (p.Person != null &&
             (EF.Functions.Like(p.Person.FullName.ToLower(), pattern) ||
              (p.Person.NationalNo != null &&
               EF.Functions.Like(p.Person.NationalNo.ToLower(), pattern)) ||
              (p.Person.Phone != null &&
               EF.Functions.Like(p.Person.Phone.ToLower(), pattern)))));
    }

    // ---------------- SORTING ----------------
    private static IQueryable<Patient> ApplySorting(
        IQueryable<Patient> query,
        GetPatientsQuery q)
    {
        var col = q.SortColumn?.Trim().ToLowerInvariant() ?? "fullname";
        var isDesc = string.Equals(q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "patientid" => isDesc
                ? query.OrderByDescending(p => p.Id)
                : query.OrderBy(p => p.Id),

            "fullname" => isDesc
                ? query.OrderByDescending(p => p.Person!.FullName)
                : query.OrderBy(p => p.Person!.FullName),

            "birthdate" => isDesc
                ? query.OrderByDescending(p => p.Person!.Birthdate)
                : query.OrderBy(p => p.Person!.Birthdate),

            "patienttype" => isDesc
                ? query.OrderByDescending(p => p.PatientType)
                : query.OrderBy(p => p.PatientType),

            "autoregistrationnumber" or "autoreg" => isDesc
                ? query.OrderByDescending(p => p.Person!.AutoRegistrationNumber)
                : query.OrderBy(p => p.Person!.AutoRegistrationNumber),

            _ => isDesc
                ? query.OrderByDescending(p => p.Person!.FullName)
                : query.OrderBy(p => p.Person!.FullName),
        };
    }
}