using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.DamageReplacementTherapy;

public sealed record DamageReplacementTherapyCommand(
    int TicketId,
    int TherapyCardId) : IRequest<Result<Success>>;