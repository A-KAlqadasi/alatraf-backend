
using AlatrafClinic.Application.Features.People.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Doctors.Commands.CreateDoctor;

public sealed record CreateDoctorCommand(
     string Fullname,
     DateTime Birthdate,
     string Phone,
     string? NationalNo,
     string Address,

     string Specialization,
     int DepartmentId
 ) : IRequest<Result<DoctorDto>>;
