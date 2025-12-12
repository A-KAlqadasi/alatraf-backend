using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.CreateTherapySession;

public sealed record class SessionProgramData(int DiagnosisProgramId, int DocotorId, int SectionId, int RoomId) : IRequest<Result<Success>>;