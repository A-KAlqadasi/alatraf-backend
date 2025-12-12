using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.IndustrialParts.Dtos;
using AlatrafClinic.Domain.Common.Results;


namespace AlatrafClinic.Application.Features.IndustrialParts.Queries.GetIndustrialParts;

public sealed record GetIndustrialPartsQuery(
) : ICachedQuery<Result<List<IndustrialPartDto>>>
{
    public string CacheKey => "industrial-parts";

    public string[] Tags => ["industrial-part"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(20);
}