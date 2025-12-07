using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.CreateTherapySession;

public sealed record class SessionProgramData(int DiagnosisProgramId, int DoctorSectionRoomId) : IRequest<Result<Success>>;
