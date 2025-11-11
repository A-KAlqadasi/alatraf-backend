using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Doctors.Commands.EndDoctorAssignment;

public sealed record EndDoctorAssignmentCommand(
    int DoctorId
) : IRequest<Result<Updated>>;
