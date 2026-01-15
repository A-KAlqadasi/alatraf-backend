using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.IndustrialParts.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.IndustrialParts;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.IndustrialParts.Queries.GetIndustrialPartsWithFilters;

public sealed class GetIndustrialPartsWithFiltersQueryHandler
    : IRequestHandler<GetIndustrialPartsWithFiltersQuery, Result<PaginatedList<IndustrialPartDto>>>
{
    private readonly IAppDbContext _context;

    public GetIndustrialPartsWithFiltersQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<IndustrialPartDto>>> Handle(
        GetIndustrialPartsWithFiltersQuery query,
        CancellationToken ct)
    {
        IQueryable<IndustrialPart> partsQuery = _context.IndustrialParts
        .Include(i=> i.IndustrialPartUnits)
            .ThenInclude(i=> i.Unit)
            .AsNoTracking();

        partsQuery = ApplySearch(partsQuery, query);
        partsQuery = ApplySorting(partsQuery, query);

        var totalCount = await partsQuery.CountAsync(ct);

        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;
        var skip = (page - 1) * pageSize;

        var items = await partsQuery
            .Skip(skip)
            .Take(pageSize)
            .Select(p => new IndustrialPartDto
            {
                IndustrialPartId = p.Id,
                Name = p.Name,
                Description = p.Description
            })
            .ToListAsync(ct);

        return new PaginatedList<IndustrialPartDto>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    private static IQueryable<IndustrialPart> ApplySearch(
        IQueryable<IndustrialPart> query,
        GetIndustrialPartsWithFiltersQuery q)
    {
        if (string.IsNullOrWhiteSpace(q.SearchTerm))
            return query;

        var term = q.SearchTerm.Trim();

        if (int.TryParse(term, out var id))
        {
            return query.Where(p => p.Id == id);
        }

        var pattern = $"%{term.ToLowerInvariant()}%";
        return query.Where(p =>
            EF.Functions.Like(p.Name.ToLower(), pattern)
            ||
            (p.Description != null && EF.Functions.Like(p.Description.ToLower(), pattern))
            );
    }

    private static IQueryable<IndustrialPart> ApplySorting(
        IQueryable<IndustrialPart> query,
        GetIndustrialPartsWithFiltersQuery q)
    {
        var col = q.SortColumn?.Trim().ToLowerInvariant() ?? "id";
        var isDesc = string.Equals(q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "id" => isDesc
                ? query.OrderByDescending(p => p.Id)
                : query.OrderBy(p => p.Id),

            "name" => isDesc
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name),

            _ => isDesc
                ? query.OrderByDescending(p => p.Id)
                : query.OrderBy(p => p.Id),
        };
    }
}