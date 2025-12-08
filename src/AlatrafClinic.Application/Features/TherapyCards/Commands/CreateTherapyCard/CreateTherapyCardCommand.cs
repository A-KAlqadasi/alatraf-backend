using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises.DiagnosisPrograms;
using AlatrafClinic.Domain.TherapyCards.Enums;

using MediatR;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.CreateTherapyCard;

public sealed record CreateTherapyCardCommand(
    int TicketId,
    string DiagnosisText,
    DateTime InjuryDate,
    List<int> InjuryReasons,
    List<int> InjurySides,
    List<int> InjuryTypes,
    DateTime ProgramStartDate,
    DateTime ProgramEndDate,
    TherapyCardType TherapyCardType,
    List<CreateTherapyCardMedicalProgramCommand> Programs,
    string? Notes = null
) : IRequest<Result<TherapyCardDiagnosisDto>>;
