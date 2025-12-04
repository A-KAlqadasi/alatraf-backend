using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.CreateRawOrder;

public sealed record CreateRawOrderCommand(int SectionId) : IRequest<Result<OrderDto>>;
