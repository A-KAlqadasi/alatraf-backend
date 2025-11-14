using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Units.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Inventory.Units.Queries.GetUnitsListQuery;

public sealed record GetUnitsListQuery : ICachedQuery<Result<List<UnitDto>>>
{
    public string CacheKey => "units_all";
    public string[] Tags => ["unit"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
