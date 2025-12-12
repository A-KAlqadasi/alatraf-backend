using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.UpdateRepairCard;

public sealed record UpdateRepairCardIndustrialPartCommand(
    int IndustrialPartId,
    int UnitId,
    int Quantity
) : IRequest<Result<Success>>;
