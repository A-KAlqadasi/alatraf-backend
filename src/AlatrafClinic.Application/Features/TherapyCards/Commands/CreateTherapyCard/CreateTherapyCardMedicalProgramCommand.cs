using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.CreateTherapyCard;

public sealed record class CreateTherapyCardMedicalProgramCommand(
    int MedicalProgramId,
    int Duration,
    string? Notes
) : IRequest<Result<Success>>;
