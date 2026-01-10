using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetDraftOrders;

public sealed record GetDraftOrdersQuery()
    : MediatR.IRequest<Result<List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>>>;
