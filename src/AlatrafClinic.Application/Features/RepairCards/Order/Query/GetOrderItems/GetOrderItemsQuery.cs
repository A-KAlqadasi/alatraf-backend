using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetOrderItems;

public sealed record GetOrderItemsQuery(int OrderId)
    : MediatR.IRequest<Result<List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderItemDto>>>,
      AlatrafClinic.Application.Common.Interfaces.ICachedQuery<Result<List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderItemDto>>>
{
    public string CacheKey => $"order_items_{OrderId}";
    public string[] Tags => new[] { "order" };
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
