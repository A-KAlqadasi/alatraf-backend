
using AlatrafClinic.Application.Features.People.Doctors.Dtos;
using AlatrafClinic.Application.Features.People.Persons.Services;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Doctors.Commands.CreateDoctor;

public sealed record CreateDoctorCommand(
    PersonInput Person,

     string Specialization,
     int DepartmentId
 ) : IRequest<Result<DoctorDto>>;
