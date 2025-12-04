using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Queries.GetExchangeOrderById;

public sealed record GetExchangeOrderByIdQuery(int ExchangeOrderId) : IRequest<Result<AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Dtos.ExchangeOrderDto>>;
