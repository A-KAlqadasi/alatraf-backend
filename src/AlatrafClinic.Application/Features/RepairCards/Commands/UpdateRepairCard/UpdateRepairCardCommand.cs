using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.UpdateRepairCard;

public sealed record UpdateRepairCardCommand(
    int RepairCardId,
    int TicketId,
    string DiagnosisText,
    DateTime InjuryDate,
    List<int> InjuryReasons,
    List<int> InjurySides,
    List<int> InjuryTypes,
    List<UpdateRepairCardIndustrialPartCommand> IndustrialParts,
    string? Notes = null
) : IRequest<Result<Updated>>;