using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetOrderItems;

public sealed record GetOrderItemsQuery(int OrderId)
    : IRequest<Result<List<OrderItemDto>>>;