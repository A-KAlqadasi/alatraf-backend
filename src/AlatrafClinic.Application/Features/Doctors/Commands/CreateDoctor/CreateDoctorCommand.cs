using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Doctors.Commands.CreateDoctor;

public sealed record CreateDoctorCommand(
    string Fullname,
    DateOnly Birthdate,
    string Phone,
    string NationalNo,
    int AddressId,
    bool Gender,
    string Specialization,
    int DepartmentId
 ) : IRequest<Result<DoctorDto>>;
