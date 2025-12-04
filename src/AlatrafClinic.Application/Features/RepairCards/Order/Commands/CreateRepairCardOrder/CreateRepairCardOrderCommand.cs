using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.CreateRepairCardOrder;

public sealed record CreateRepairCardOrderCommand(int RepairCardId, int SectionId) : IRequest<Result<OrderDto>>;
