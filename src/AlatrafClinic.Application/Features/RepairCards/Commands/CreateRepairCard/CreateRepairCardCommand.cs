using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.CreateRepairCard;

public sealed record CreateRepairCardCommand(
    int TicketId,
    string DiagnosisText,
    DateTime InjuryDate,
    List<int> InjuryReasons,
    List<int> InjurySides,
    List<int> InjuryTypes,
    int PatientId,
    List<(int industrialPartId, int unitId, int quantity, decimal price)> IndustrialParts,
    string? Notes = null
) : IRequest<Result<RepairCardDto>>;