using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.CreateRepairCard;

public sealed record CreateRepairCardIndustrialPartCommand(
    int IndustrialPartId,
    int UnitId,
    int Quantity,
    decimal Price
) : IRequest<Result<Success>>;
