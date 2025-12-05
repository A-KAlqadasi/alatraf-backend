using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetDraftOrders;

public sealed record GetDraftOrdersQuery()
    : MediatR.IRequest<Result<List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>>>,
      AlatrafClinic.Application.Common.Interfaces.ICachedQuery<Result<List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>>>
{
    public string CacheKey => "orders_drafts";
    public string[] Tags => new[] { "order" };
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
