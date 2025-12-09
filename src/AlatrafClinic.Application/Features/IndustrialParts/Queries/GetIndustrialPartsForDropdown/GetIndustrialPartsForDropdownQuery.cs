
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.IndustrialParts.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.IndustrialParts.Queries.GetIndustrialPartsForDropdown;

public sealed record GetIndustrialPartsForDropdownQuery : ICachedQuery<Result<List<IndustrialPartDto>>>
{
    public string CacheKey => "get-industrial-parts";

    public string[] Tags => ["industrial-part"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(20);
}