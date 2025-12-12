using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards.Enums;

using MediatR;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.RenewTherapyCard;

public sealed record class RenewTherapyCardCommand(
    int TherapyCardId,
    int TicketId,
    DateOnly ProgramStartDate,
    DateOnly ProgramEndDate,
    TherapyCardType TherapyCardType,
    List<RenewTherapyCardMedicalProgramCommand> Programs,
    string? Notes = null
) : IRequest<Result<TherapyCardDiagnosisDto>>;
