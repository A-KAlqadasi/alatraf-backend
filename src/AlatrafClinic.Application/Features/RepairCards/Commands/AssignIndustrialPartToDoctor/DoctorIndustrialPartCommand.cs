using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.AssignIndustrialPartToDoctor;

public sealed record DoctorIndustrialPartCommand(
    int DiagnosisIndustrialPartId,
    int DoctorId, int SectionId) : IRequest<Result<Success>>;