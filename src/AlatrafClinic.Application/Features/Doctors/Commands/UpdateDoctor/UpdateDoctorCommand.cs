using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Doctors.Commands.UpdateDoctor;

public sealed record UpdateDoctorCommand(
    int DoctorId,
    string Fullname,
    DateOnly Birthdate,
    string Phone,
    string NationalNo,
    string Address,
    bool Gender,
    string Specialization,
    int DepartmentId
   ) : IRequest<Result<Updated>>;
