using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.AssignIndustrialPartToDoctor;

public sealed record AssignIndustrialPartToDoctorCommand(
    int RepairCardId,
    List<DoctorIndustrialPartCommand> DoctorIndustrialParts) : IRequest<Result<Updated>>;