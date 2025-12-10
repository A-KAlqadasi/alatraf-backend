using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.IndustrialParts.Dtos;
using AlatrafClinic.Domain.Common.Results;


namespace AlatrafClinic.Application.Features.IndustrialParts.Queries.GetIndustrialParts;

public sealed record GetIndustrialPartsQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null
) : ICachedQuery<Result<PaginatedList<IndustrialPartDto>>>
{
    public string CacheKey =>
        $"industrialparts-dropdown:p={Page}:ps={PageSize}" +
        $":q={(SearchTerm ?? "-")}";

    public string[] Tags => ["industrial-part"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(20);
}