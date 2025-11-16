using AlatrafClinic.Application.Common.Interfaces.Repositories;
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
    private readonly IUnitOfWork _unitOfWork;

    public GetIndustrialPartsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<IndustrialPartDto>>> Handle(GetIndustrialPartsQuery query, CancellationToken ct)
    {
        var partsQuery = await _unitOfWork.IndustrialParts.GetIndustrialPartsQueryAsync(ct);

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            partsQuery = ApplySearch(partsQuery, query.SearchTerm!);

        partsQuery = ApplySorting(partsQuery, query.SortColumn, query.SortDirection);

        // Paging guards
        var page = query.Page < 1 ? 1 : query.Page;
        var size = query.PageSize < 1 ? 10 : query.PageSize;

        var count = await partsQuery.CountAsync(ct);

        var items = await partsQuery
            .Skip((page - 1) * size)
            .Take(size)
            .Select(p => new IndustrialPartDto
            {
                IndustrialPartId = p.Id,
                Name = p.Name,
                Description = p.Description,
                IndustrialPartUnits = p.IndustrialPartUnits.ToDtos()
            })
            .ToListAsync(ct);

        return new PaginatedList<IndustrialPartDto>
        {
            Items = items,
            PageNumber = page,
            PageSize = size,
            TotalCount = count,
            TotalPages = (int)Math.Ceiling(count / (double)size)
        };
    }

    // ---------------- SEARCH ----------------
    private static IQueryable<IndustrialPart> ApplySearch(IQueryable<IndustrialPart> query, string term)
    {
        var pattern = $"%{term.Trim().ToLower()}%";

        return query.Where(p =>
            EF.Functions.Like(p.Name.ToLower(), pattern) ||
            (p.Description != null && EF.Functions.Like(p.Description.ToLower(), pattern)) ||
            p.IndustrialPartUnits.Any(u =>
                EF.Functions.Like(u.Unit!.Name.ToLower(), pattern))
        );
    }

    // ---------------- SORTING ----------------
    private static IQueryable<IndustrialPart> ApplySorting(IQueryable<IndustrialPart> query, string sortColumn, string sortDirection)
    {
        var col = sortColumn?.Trim().ToLowerInvariant() ?? "name";
        var isDesc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "name" => isDesc ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),

            "description" => isDesc
                ? query.OrderByDescending(p => p.Description)
                : query.OrderBy(p => p.Description),

            "unitcount" => isDesc
                ? query.OrderByDescending(p => p.IndustrialPartUnits.Count)
                : query.OrderBy(p => p.IndustrialPartUnits.Count),

            _ => query.OrderBy(p => p.Name)
        };
    }
}
