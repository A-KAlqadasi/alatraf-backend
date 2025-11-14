using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Inventory.Items.Queries.GetInactiveItemsQuery;

public sealed record GetInactiveItemsQuery : ICachedQuery<Result<List<ItemDto>>>
{
    public string CacheKey => "inactive_items";
    public string[] Tags => ["item"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}