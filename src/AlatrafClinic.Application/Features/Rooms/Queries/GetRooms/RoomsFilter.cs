using AlatrafClinic.Application.Common.Specifications;
using AlatrafClinic.Domain.Departments.Sections.Rooms;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Rooms.Queries.GetRooms;

public sealed class RoomsFilter : FilterSpecification<Room>
{
    private readonly GetRoomsQuery _q;

    public RoomsFilter(GetRoomsQuery q)
        : base(q.Page, q.PageSize)
    {
        _q = q;
    }

    public override IQueryable<Room> Apply(IQueryable<Room> query)
    {
        query = query
            .Include(r => r.Section)
                .ThenInclude(s => s.Department)
            .Include(r => r.DoctorAssignments)
                .ThenInclude(dsr => dsr.Doctor);

        query = ApplyFilters(query);
        query = ApplySearch(query);
        query = ApplySorting(query);

        return query;
    }

    // ---------------- FILTERS ----------------
    private IQueryable<Room> ApplyFilters(IQueryable<Room> query)
    {
        if (_q.SectionId.HasValue && _q.SectionId.Value > 0)
        {
            var sectionId = _q.SectionId.Value;
            query = query.Where(r => r.SectionId == sectionId);
        }

        if (_q.DepartmentId.HasValue && _q.DepartmentId.Value > 0)
        {
            var depId = _q.DepartmentId.Value;
            query = query.Where(r => r.Section.DepartmentId == depId);
        }

        if (_q.DoctorId.HasValue && _q.DoctorId.Value > 0)
        {
            var doctorId = _q.DoctorId.Value;
            query = query.Where(r =>
                r.DoctorAssignments.Any(dsr => dsr.DoctorId == doctorId));
        }

        if (_q.OnlyActiveAssignments.HasValue)
        {
            if (_q.OnlyActiveAssignments.Value)
            {
                // rooms that currently have an active doctor assignment
                query = query.Where(r =>
                    r.DoctorAssignments.Any(dsr => dsr.IsActive));
            }
            else
            {
                // rooms that do NOT have any active assignment
                query = query.Where(r =>
                    !r.DoctorAssignments.Any(dsr => dsr.IsActive));
            }
        }

        return query;
    }

    // ---------------- SEARCH ----------------
    private IQueryable<Room> ApplySearch(IQueryable<Room> query)
    {
        if (string.IsNullOrWhiteSpace(_q.SearchTerm))
            return query;

        var pattern = $"%{_q.SearchTerm!.Trim().ToLower()}%";

        return query.Where(r =>
            EF.Functions.Like(r.Name.ToLower(), pattern) ||
            EF.Functions.Like(r.Section.Name.ToLower(), pattern) ||
            EF.Functions.Like(r.Section.Department.Name.ToLower(), pattern));
    }

    // ---------------- SORTING ----------------
    private IQueryable<Room> ApplySorting(IQueryable<Room> query)
    {
        var col = _q.SortColumn?.Trim().ToLowerInvariant() ?? "name";
        var isDesc = string.Equals(_q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "id" => isDesc
                ? query.OrderByDescending(r => r.Id)
                : query.OrderBy(r => r.Id),

            "name" => isDesc
                ? query.OrderByDescending(r => r.Name)
                : query.OrderBy(r => r.Name),

            "section" => isDesc
                ? query.OrderByDescending(r => r.Section.Name)
                : query.OrderBy(r => r.Section.Name),

            "department" => isDesc
                ? query.OrderByDescending(r => r.Section.Department.Name)
                : query.OrderBy(r => r.Section.Department.Name),

            _ => isDesc
                ? query.OrderByDescending(r => r.Name)
                : query.OrderBy(r => r.Name),
        };
    }
}
