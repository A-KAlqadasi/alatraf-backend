using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetOrdersBySection;

public sealed record GetOrdersBySectionQuery(int SectionId)
    : MediatR.IRequest<Result<List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>>>,
      AlatrafClinic.Application.Common.Interfaces.ICachedQuery<Result<List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>>>
{
    public string CacheKey => $"orders_section_{SectionId}";
    public string[] Tags => new[] { "order", "section" };
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
