using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards.Enums;

using MediatR;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.UpdateTherapyCard;

public sealed record class UpdateTherapyCardCommand(
    int TherapyCardId,
    int TicketId,
    string DiagnosisText,
    DateOnly InjuryDate,
    List<int> InjuryReasons,
    List<int> InjurySides,
    List<int> InjuryTypes,
    DateOnly ProgramStartDate,
    DateOnly ProgramEndDate,
    TherapyCardType TherapyCardType,
    List<UpdateTherapyCardMedicalProgramCommand> Programs,
    string? Notes = null
) : IRequest<Result<Updated>>;
