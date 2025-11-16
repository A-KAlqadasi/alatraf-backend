using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Inventory.Items.Queries.GetItemsListQuery;

public sealed record GetItemsListQuery : ICachedQuery<Result<List<ItemDto>>>
{
    public string CacheKey => "items_all";
    public string[] Tags => ["item"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
