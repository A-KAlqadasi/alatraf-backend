using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetOrdersList;

public sealed record GetOrdersListQuery() : IRequest<Result<List<OrderDto>>>;