
using AlatrafClinic.Application.Features.People.Persons.Services;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Doctors.Commands.UpdateDoctor;

public sealed record UpdateDoctorCommand(
       int DoctorId,
       PersonInput Person,
       string? Specialization
   ) : IRequest<Result<Updated>>;
