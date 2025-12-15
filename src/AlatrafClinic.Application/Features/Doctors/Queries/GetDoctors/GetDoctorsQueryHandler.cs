using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.People.Doctors;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetDoctors;

public class GetDoctorsQueryHandler
    : IRequestHandler<GetDoctorsQuery, Result<PaginatedList<DoctorListItemDto>>>
{
    private readonly IAppDbContext _context;

    public GetDoctorsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<DoctorListItemDto>>> Handle(
        GetDoctorsQuery query,
        CancellationToken ct)
    {
        IQueryable<Doctor> doctorsQuery = _context.Doctors
            .Include(d => d.Person)
            .Include(d => d.Department)
            .Include(d => d.Assignments)
                .ThenInclude(a => a.Section)
            .Include(d => d.Assignments)
                .ThenInclude(a => a.Room)
            .AsNoTracking();

        doctorsQuery = ApplyFilters(doctorsQuery, query);
        doctorsQuery = ApplySearch(doctorsQuery, query);
        doctorsQuery = ApplySorting(doctorsQuery, query);

        var totalCount = await doctorsQuery.CountAsync(ct);

        var page = query.Page;
        var pageSize = query.PageSize;
        var skip = (page - 1) * pageSize;

        // Paging
        var doctors = await doctorsQuery
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        // Projection – same logic as you had
        var items = doctors
            .Select(d => new DoctorListItemDto
            {
                DoctorId      = d.Id,
                FullName      = d.Person != null ? d.Person.FullName : string.Empty,
                Specialization = d.Specialization,
                DepartmentId  = d.DepartmentId,
                DepartmentName = d.Department.Name,

                SectionId = d.Assignments
                    .Where(a => a.IsActive)
                    .Select(a => a.SectionId)
                    .FirstOrDefault(),

                SectionName = d.Assignments
                    .Where(a => a.IsActive)
                    .Select(a => a.Section.Name)
                    .FirstOrDefault(),

                RoomId = d.Assignments
                    .Where(a => a.IsActive)
                    .Select(a => a.RoomId)
                    .FirstOrDefault(),

                RoomName = d.Assignments
                    .Where(a => a.IsActive && a.Room != null)
                    .Select(a => a.Room!.Name ?? string.Empty)
                    .FirstOrDefault(),

                AssignDate = d.Assignments
                    .Where(a => a.IsActive)
                    .Select(a => a.AssignDate)
                    .FirstOrDefault(),

                IsActiveAssignment = d.Assignments.Any(a => a.IsActive)
            })
            .ToList();

        return new PaginatedList<DoctorListItemDto>
        {
            Items      = items,
            PageNumber = page,
            PageSize   = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    private static IQueryable<Doctor> ApplyFilters(
        IQueryable<Doctor> query,
        GetDoctorsQuery q)
    {
        if (q.DepartmentId.HasValue)
            query = query.Where(d => d.DepartmentId == q.DepartmentId.Value);

        if (!string.IsNullOrWhiteSpace(q.Specialization))
        {
            var spec = q.Specialization.Trim().ToLower();
            query = query.Where(d =>
                d.Specialization != null &&
                EF.Functions.Like(d.Specialization.ToLower(), $"%{spec}%"));
        }

        if (q.SectionId.HasValue)
        {
            var sectionId = q.SectionId.Value;
            query = query.Where(d =>
                d.Assignments.Any(a => a.IsActive && a.SectionId == sectionId));
        }

        if (q.RoomId.HasValue)
        {
            var roomId = q.RoomId.Value;
            query = query.Where(d =>
                d.Assignments.Any(a => a.IsActive && a.RoomId == roomId));
        }

        if (q.HasActiveAssignment.HasValue)
        {
            if (q.HasActiveAssignment.Value)
            {
                query = query.Where(d => d.Assignments.Any(a => a.IsActive));
            }
            else
            {
                query = query.Where(d => d.Assignments.All(a => !a.IsActive));
            }
        }

        return query;
    }

    private static IQueryable<Doctor> ApplySearch(
        IQueryable<Doctor> query,
        GetDoctorsQuery q)
    {
        if (string.IsNullOrWhiteSpace(q.Search))
            return query;

        var pattern = $"%{q.Search!.Trim().ToLower()}%";

        return query.Where(d =>
            (d.Person != null &&
             EF.Functions.Like(d.Person.FullName.ToLower(), pattern))
            ||
            (d.Specialization != null &&
             EF.Functions.Like(d.Specialization.ToLower(), pattern))
        );
    }

    private static IQueryable<Doctor> ApplySorting(
        IQueryable<Doctor> query,
        GetDoctorsQuery q)
    {
        var col  = q.SortBy?.Trim().ToLowerInvariant() ?? "assigndate";
        var desc = string.Equals(q.SortDir, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "name" => desc
                ? query.OrderByDescending(d => d.Person!.FullName)
                : query.OrderBy(d => d.Person!.FullName),

            "department" => desc
                ? query.OrderByDescending(d => d.Department.Name)
                : query.OrderBy(d => d.Department.Name),

            "specialization" => desc
                ? query.OrderByDescending(d => d.Specialization)
                : query.OrderBy(d => d.Specialization),

            "assigndate" => desc
                ? query.OrderByDescending(d =>
                    d.Assignments
                        .Where(a => a.IsActive)
                        .Select(a => a.AssignDate)
                        .FirstOrDefault())
                : query.OrderBy(d =>
                    d.Assignments
                        .Where(a => a.IsActive)
                        .Select(a => a.AssignDate)
                        .FirstOrDefault()),

            _ => query.OrderByDescending(d => d.Person!.FullName)
        };
    }
}