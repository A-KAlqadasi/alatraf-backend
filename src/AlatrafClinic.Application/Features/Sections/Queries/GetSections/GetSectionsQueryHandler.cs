using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Sections.Dtos;
using AlatrafClinic.Application.Features.Sections.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.Sections;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Sections.Queries.GetSections;

public class GetSectionsQueryHandler
    : IRequestHandler<GetSectionsQuery, Result<PaginatedList<SectionDto>>>
{
    private readonly IAppDbContext _context;

    public GetSectionsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<SectionDto>>> Handle(
        GetSectionsQuery query,
        CancellationToken ct)
    {
        IQueryable<Section> sectionsQuery = _context.Sections
            .Include(s => s.Department)
            .Include(s => s.Rooms)
            .AsNoTracking();

        sectionsQuery = ApplySearch(sectionsQuery, query);
        sectionsQuery = ApplySorting(sectionsQuery, query);

        var totalCount = await sectionsQuery.CountAsync(ct);

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 10 : query.PageSize;
        var skip = (page - 1) * pageSize;

        var sections = await sectionsQuery
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        var items = sections.ToDtos();

        return new PaginatedList<SectionDto>
        {
            Items      = items,
            PageNumber = page,
            PageSize   = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    private static IQueryable<Section> ApplySearch(
        IQueryable<Section> query,
        GetSectionsQuery q)
    {
        if (string.IsNullOrWhiteSpace(q.SearchTerm))
            return query;

        var pattern = $"%{q.SearchTerm!.Trim().ToLower()}%";

        return query.Where(s =>
            EF.Functions.Like(s.Name.ToLower(), pattern) ||
            EF.Functions.Like(s.Department.Name.ToLower(), pattern));
    }

    private static IQueryable<Section> ApplySorting(
        IQueryable<Section> query,
        GetSectionsQuery q)
    {
        var col = q.SortColumn?.Trim().ToLowerInvariant() ?? "name";
        var isDesc = string.Equals(q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

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
