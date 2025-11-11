using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.GenerateSessions;

public sealed record TakeSessionCommand(
    int TherapyCardId,
    int SessionId,
    List<(int diagnosisProgramId, int doctorSectionRoomId)> SessionProgramsData) : IRequest<Result<Success>>;