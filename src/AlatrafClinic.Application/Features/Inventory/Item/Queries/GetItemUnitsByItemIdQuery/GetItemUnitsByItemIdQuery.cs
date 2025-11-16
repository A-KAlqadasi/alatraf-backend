using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Inventory.Items.Queries.GetItemUnitsByItemIdQuery;

public sealed record GetItemUnitsByItemIdQuery(int ItemId) 
    : ICachedQuery<Result<List<ItemUnitDto>>>
{
    public string CacheKey => $"item_units_{ItemId}";
    public string[] Tags => ["itemunit", $"item_{ItemId}"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
