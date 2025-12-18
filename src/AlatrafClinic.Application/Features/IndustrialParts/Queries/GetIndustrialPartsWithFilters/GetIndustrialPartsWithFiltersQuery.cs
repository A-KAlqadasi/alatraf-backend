using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.IndustrialParts.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.IndustrialParts.Queries.GetIndustrialPartsWithFilters;

public sealed record GetIndustrialPartsWithFiltersQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    string SortColumn = "Id",
    string SortDirection = "desc"
) : ICachedQuery<Result<PaginatedList<IndustrialPartDto>>>
{
    public string CacheKey =>
        $"industrialparts:p={Page}:ps={PageSize}" +
        $":q={(SearchTerm ?? "-")}" +
        $":sort={SortColumn}:{SortDirection}";

    public string[] Tags => ["industrial-part"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(20);
}
