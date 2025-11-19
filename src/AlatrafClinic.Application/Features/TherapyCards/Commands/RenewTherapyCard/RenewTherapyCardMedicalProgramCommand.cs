using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.RenewTherapyCard;

public sealed record class RenewTherapyCardMedicalProgramCommand(
    int MedicalProgramId,
    int Duration,
    string? Notes = null
) : IRequest<Result<Updated>>;
