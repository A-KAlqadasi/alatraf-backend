using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.AssignRepairCardToDoctor;

public sealed record AssignRepairCardToDoctorCommand(int RepairCardId, int DoctorSectionRoomId) : IRequest<Result<Updated>>;