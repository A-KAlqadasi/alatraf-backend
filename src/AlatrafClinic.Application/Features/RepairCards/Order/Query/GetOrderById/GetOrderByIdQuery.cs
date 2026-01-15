using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetOrderById;

public sealed record GetOrderByIdQuery(int OrderId) : IRequest<Result<OrderDto>>;
