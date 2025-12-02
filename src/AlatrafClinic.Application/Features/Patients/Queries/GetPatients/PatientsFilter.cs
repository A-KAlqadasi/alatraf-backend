using AlatrafClinic.Application.Common.Specifications;
using AlatrafClinic.Domain.Patients;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Patients.Queries.GetPatients;

public sealed class PatientsFilter : FilterSpecification<Patient>
{
    private readonly GetPatientsQuery _q;

    public PatientsFilter(GetPatientsQuery q)
        : base(q.Page, q.PageSize)
    {
        _q = q;
    }

    public override IQueryable<Patient> Apply(IQueryable<Patient> query)
    {
        query = query
            .Include(p => p.Person);

        query = ApplyFilters(query);
        query = ApplySearch(query);
        query = ApplySorting(query);

        return query;
    }

    // ---------------- FILTERS ----------------
    private IQueryable<Patient> ApplyFilters(IQueryable<Patient> query)
    {
        if (_q.PatientType.HasValue)
        {
            var type = _q.PatientType.Value;
            query = query.Where(p => p.PatientType == type);
        }

        if (_q.Gender.HasValue)
        {
            var gender = _q.Gender.Value;
            query = query.Where(p => p.Person != null && p.Person.Gender == gender);
        }

        if (_q.BirthdateFrom.HasValue)
        {
            var from = _q.BirthdateFrom.Value.Date;
            query = query.Where(p => p.Person != null && p.Person.Birthdate >= from);
        }

        if (_q.BirthdateTo.HasValue)
        {
            var to = _q.BirthdateTo.Value.Date;
            query = query.Where(p => p.Person != null && p.Person.Birthdate <= to);
        }

        if (_q.HasNationalNo.HasValue)
        {
            if (_q.HasNationalNo.Value)
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
    private IQueryable<Patient> ApplySearch(IQueryable<Patient> query)
    {
        if (string.IsNullOrWhiteSpace(_q.SearchTerm))
            return query;

        var pattern = $"%{_q.SearchTerm!.Trim().ToLower()}%";

        return query.Where(p =>
            EF.Functions.Like(p.AutoRegistrationNumber!.ToLower(), pattern) ||
            (p.Person != null &&
             (EF.Functions.Like(p.Person.FullName.ToLower(), pattern) ||
              (p.Person.NationalNo != null &&
               EF.Functions.Like(p.Person.NationalNo.ToLower(), pattern)) ||
              (p.Person.Phone != null &&
               EF.Functions.Like(p.Person.Phone.ToLower(), pattern)))));
    }

    // ---------------- SORTING ----------------
    private IQueryable<Patient> ApplySorting(IQueryable<Patient> query)
    {
        var col = _q.SortColumn?.Trim().ToLowerInvariant() ?? "fullname";
        var isDesc = string.Equals(_q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

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
                ? query.OrderByDescending(p => p.AutoRegistrationNumber)
                : query.OrderBy(p => p.AutoRegistrationNumber),

            _ => isDesc
                ? query.OrderByDescending(p => p.Person!.FullName)
                : query.OrderBy(p => p.Person!.FullName),
        };
    }
}
