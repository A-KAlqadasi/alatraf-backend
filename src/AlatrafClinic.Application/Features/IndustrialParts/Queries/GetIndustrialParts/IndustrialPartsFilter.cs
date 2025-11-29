using AlatrafClinic.Application.Common.Specifications;
using AlatrafClinic.Domain.RepairCards.IndustrialParts;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.IndustrialParts.Queries.GetIndustrialParts;

public sealed class IndustrialPartsFilter : FilterSpecification<IndustrialPart>
{
    private readonly GetIndustrialPartsQuery _q;

    public IndustrialPartsFilter(GetIndustrialPartsQuery q)
        : base(q.Page, q.PageSize)
    {
        _q = q;
    }

    public override IQueryable<IndustrialPart> Apply(IQueryable<IndustrialPart> query)
    {
        // Includes required for search/sorting
        query = query
            .Include(p => p.IndustrialPartUnits)
                .ThenInclude(u => u.Unit);
                

        query = ApplySearch(query);
        query = ApplySorting(query);

        return query;
    }

    // --------------- SEARCH ---------------
    private IQueryable<IndustrialPart> ApplySearch(IQueryable<IndustrialPart> query)
    {
        if (string.IsNullOrWhiteSpace(_q.SearchTerm))
            return query;

        var pattern = $"%{_q.SearchTerm!.Trim().ToLower()}%";

        return query.Where(p =>
            EF.Functions.Like(p.Name.ToLower(), pattern) ||
            (p.Description != null && EF.Functions.Like(p.Description.ToLower(), pattern)) ||
            p.IndustrialPartUnits.Any(u =>
                u.Unit != null &&
                EF.Functions.Like(u.Unit.Name.ToLower(), pattern))
        );
    }

    // --------------- SORTING ---------------
    private IQueryable<IndustrialPart> ApplySorting(IQueryable<IndustrialPart> query)
    {
        var col = _q.SortColumn?.Trim().ToLowerInvariant() ?? "name";
        var isDesc = string.Equals(_q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "name" => isDesc
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name),

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
