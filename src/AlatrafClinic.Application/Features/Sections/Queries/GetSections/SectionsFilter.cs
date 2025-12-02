using AlatrafClinic.Application.Common.Specifications;
using AlatrafClinic.Domain.Departments.Sections;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Sections.Queries.GetSections;

public sealed class SectionsFilter : FilterSpecification<Section>
{
    private readonly GetSectionsQuery _q;

    public SectionsFilter(GetSectionsQuery q)
        : base(q.Page, q.PageSize)
    {
        _q = q;
    }

    public override IQueryable<Section> Apply(IQueryable<Section> query)
    {
        query = query
            .Include(s => s.Department)
            .Include(s => s.Rooms)
            .Include(s => s.DoctorAssignments)
                .ThenInclude(dsr => dsr.Doctor);

        query = ApplyFilters(query);
        query = ApplySearch(query);
        query = ApplySorting(query);

        return query;
    }

    // ---------------- FILTERS ----------------
    private IQueryable<Section> ApplyFilters(IQueryable<Section> query)
    {
        if (_q.DepartmentId.HasValue && _q.DepartmentId.Value > 0)
        {
            var depId = _q.DepartmentId.Value;
            query = query.Where(s => s.DepartmentId == depId);
        }

        if (_q.DoctorId.HasValue && _q.DoctorId.Value > 0)
        {
            var doctorId = _q.DoctorId.Value;
            query = query.Where(s =>
                s.DoctorAssignments.Any(da => da.DoctorId == doctorId));
        }

        if (_q.RoomId.HasValue && _q.RoomId.Value > 0)
        {
            var roomId = _q.RoomId.Value;
            query = query.Where(s =>
                s.Rooms.Any(r => r.Id == roomId));
        }

        return query;
    }

    // ---------------- SEARCH ----------------
    private IQueryable<Section> ApplySearch(IQueryable<Section> query)
    {
        if (string.IsNullOrWhiteSpace(_q.SearchTerm))
            return query;

        var pattern = $"%{_q.SearchTerm!.Trim().ToLower()}%";

        return query.Where(s =>
            EF.Functions.Like(s.Name.ToLower(), pattern) ||
            EF.Functions.Like(s.Department.Name.ToLower(), pattern));
    }

    // ---------------- SORTING ----------------
    private IQueryable<Section> ApplySorting(IQueryable<Section> query)
    {
        var col = _q.SortColumn?.Trim().ToLowerInvariant() ?? "name";
        var isDesc = string.Equals(_q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "id" => isDesc
                ? query.OrderByDescending(s => s.Id)
                : query.OrderBy(s => s.Id),

            "department" => isDesc
                ? query.OrderByDescending(s => s.Department.Name)
                : query.OrderBy(s => s.Department.Name),

            "name" or _ => isDesc
                ? query.OrderByDescending(s => s.Name)
                : query.OrderBy(s => s.Name),
        };
    }
}
