using AlatrafClinic.Application.Common.Specifications;
using AlatrafClinic.Domain.People.Doctors;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetDoctors;


public sealed class DoctorsFilter : FilterSpecification<Doctor>
{
    private readonly GetDoctorsQuery _q;

    public DoctorsFilter(GetDoctorsQuery q)
        : base(q.Page, q.PageSize)
    {
        _q = q;
    }

    public override IQueryable<Doctor> Apply(IQueryable<Doctor> query)
    {
        // Includes required for filters/search/sorting/projection
        query = query
            .Include(d => d.Person)
            .Include(d => d.Department)
            .Include(d => d.Assignments)
                .ThenInclude(a => a.Section)
            .Include(d => d.Assignments)
                .ThenInclude(a => a.Room);

        query = ApplyFilters(query);
        query = ApplySearch(query);
        query = ApplySorting(query);

        return query;
    }

    // ---------------- FILTERS ----------------
    private IQueryable<Doctor> ApplyFilters(IQueryable<Doctor> query)
    {
        if (_q.DepartmentId.HasValue)
            query = query.Where(d => d.DepartmentId == _q.DepartmentId.Value);

        if (!string.IsNullOrWhiteSpace(_q.Specialization))
        {
            var spec = _q.Specialization.Trim().ToLower();
            query = query.Where(d =>
                d.Specialization != null &&
                EF.Functions.Like(d.Specialization.ToLower(), $"%{spec}%"));
        }

        if (_q.SectionId.HasValue)
        {
            var sectionId = _q.SectionId.Value;
            query = query.Where(d =>
                d.Assignments.Any(a => a.IsActive && a.SectionId == sectionId));
        }

        if (_q.RoomId.HasValue)
        {
            var roomId = _q.RoomId.Value;
            query = query.Where(d =>
                d.Assignments.Any(a => a.IsActive && a.RoomId == roomId));
        }

        if (_q.HasActiveAssignment.HasValue)
        {
            if (_q.HasActiveAssignment.Value)
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

    // ---------------- SEARCH ----------------
    private IQueryable<Doctor> ApplySearch(IQueryable<Doctor> query)
    {
        if (string.IsNullOrWhiteSpace(_q.Search))
            return query;

        var pattern = $"%{_q.Search!.Trim().ToLower()}%";

        return query.Where(d =>
            (d.Person != null &&
             EF.Functions.Like(d.Person.FullName.ToLower(), pattern))
            ||
            (d.Specialization != null &&
             EF.Functions.Like(d.Specialization.ToLower(), pattern))
        );
    }

    // ---------------- SORTING ----------------
    private IQueryable<Doctor> ApplySorting(IQueryable<Doctor> query)
    {
        var col = _q.SortBy?.Trim().ToLowerInvariant() ?? "assigndate";
        var desc = string.Equals(_q.SortDir, "desc", StringComparison.OrdinalIgnoreCase);

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
