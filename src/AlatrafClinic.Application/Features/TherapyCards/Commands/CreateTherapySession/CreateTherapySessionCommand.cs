using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.CreateTherapySession;

public sealed record class CreateTherapySessionCommand(int TherapyCardId, List<(int diagnosisProgramId, int doctorSectionRoomId)> SessionProgramsData) : IRequest<Result<SessionDto>>;