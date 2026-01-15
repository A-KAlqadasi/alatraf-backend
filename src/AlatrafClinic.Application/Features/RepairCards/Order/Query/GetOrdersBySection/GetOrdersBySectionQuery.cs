using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetOrdersBySection;

public sealed record GetOrdersBySectionQuery(int SectionId)
    : IRequest<Result<List<OrderDto>>>;

