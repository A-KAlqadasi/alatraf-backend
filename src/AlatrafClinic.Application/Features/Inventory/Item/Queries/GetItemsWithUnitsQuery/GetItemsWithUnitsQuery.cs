using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Inventory.Items.Queries.GetItemsWithUnitsQuery;

public sealed record GetItemsWithUnitsQuery : ICachedQuery<Result<List<ItemDto>>>
{
    public string CacheKey => "items-with-units";
    public string[] Tags => ["item"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
