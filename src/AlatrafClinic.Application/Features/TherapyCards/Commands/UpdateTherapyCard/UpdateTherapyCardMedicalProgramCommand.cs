using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.UpdateTherapyCard;

public sealed record class UpdateTherapyCardMedicalProgramCommand(
    int MedicalProgramId,
    int Duration,
    string? Notes
) : IRequest<Result<Success>>;
