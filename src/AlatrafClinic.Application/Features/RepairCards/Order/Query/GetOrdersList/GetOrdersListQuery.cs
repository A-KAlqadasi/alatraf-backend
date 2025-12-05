using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetOrdersList;

public sealed record GetOrdersListQuery() : AlatrafClinic.Application.Common.Interfaces.ICachedQuery<Result<List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>>>
{
    public string CacheKey => "orders_all";
    public string[] Tags => new[] { "order" };
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
