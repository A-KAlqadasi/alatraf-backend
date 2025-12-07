using AlatrafClinic.Application.Features.TherapyCards.Commands.CreateTherapySession;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.GenerateSessions;

public sealed record TakeSessionCommand(
    int TherapyCardId,
    int SessionId,
    List<SessionProgramData> SessionProgramsData) : IRequest<Result<Success>>;