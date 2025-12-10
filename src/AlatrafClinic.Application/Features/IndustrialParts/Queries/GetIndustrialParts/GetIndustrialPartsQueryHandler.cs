using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.IndustrialParts.Dtos;
using AlatrafClinic.Application.Features.IndustrialParts.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards.IndustrialParts;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.IndustrialParts.Queries.GetIndustrialParts;

public sealed class GetIndustrialPartsQueryHandler
    : IRequestHandler<GetIndustrialPartsQuery, Result<PaginatedList<IndustrialPartDto>>>
{
    private readonly IAppDbContext _context;

    public GetIndustrialPartsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<IndustrialPartDto>>> Handle(
        GetIndustrialPartsQuery query,
        CancellationToken ct)
    {
        IQueryable<IndustrialPart> partsQuery = _context.IndustrialParts
            .Include(i => i.IndustrialPartUnits).ThenInclude(i=> i.Unit)
            .AsNoTracking();

        partsQuery = ApplySearch(partsQuery, query);

        var totalCount = await partsQuery.CountAsync(ct);

        if (totalCount == 0)
        {
            return IndustrialPartErrors.NoIndustrialPartsFound;
        }

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 10 : query.PageSize;
        var skip = (page - 1) * pageSize;

        var parts = await partsQuery
            .OrderBy(x => x.Name)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        var items = parts.ToDtos().ToList();

        var paged = new PaginatedList<IndustrialPartDto>
        {
            Items      = items,
            PageNumber = page,
            PageSize   = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return paged;
    }

    private static IQueryable<IndustrialPart> ApplySearch(
        IQueryable<IndustrialPart> query,
        GetIndustrialPartsQuery q)
    {
        if (string.IsNullOrWhiteSpace(q.SearchTerm))
            return query;

        var pattern = $"%{q.SearchTerm!.Trim().ToLower()}%";

        return query.Where(p =>
            EF.Functions.Like(p.Name.ToLower(), pattern) ||
            (p.Description != null &&
             EF.Functions.Like(p.Description.ToLower(), pattern))
        );
    }
}