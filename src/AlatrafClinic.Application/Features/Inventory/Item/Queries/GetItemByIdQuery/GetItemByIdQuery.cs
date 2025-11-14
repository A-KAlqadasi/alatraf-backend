using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Inventory.Items.Queries.GetItemByIdQuery;

public sealed record GetItemByIdQuery(int Id) : ICachedQuery<Result<ItemDto>>
{
    public string CacheKey => $"item-{Id}";
    public string[] Tags => ["item"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
