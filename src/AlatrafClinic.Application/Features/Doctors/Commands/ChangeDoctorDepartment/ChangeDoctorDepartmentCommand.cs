using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Doctors.Commands.ChangeDoctorDepartment;

public sealed record ChangeDoctorDepartmentCommand(
    int DoctorId,
    int NewDepartmentId
) : IRequest<Result<Updated>>;
