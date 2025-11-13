using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.AssignIndustrialPartToDoctor;

public sealed record DoctorIndustrialPartCommand(
    int DiagnosisIndustrialPartId,
    int DoctorSectionRoomId) : IRequest<Result<Success>>;