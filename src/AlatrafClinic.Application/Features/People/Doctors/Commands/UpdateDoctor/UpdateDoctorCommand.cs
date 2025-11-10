
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Doctors.Commands.UpdateDoctor;

public sealed record UpdateDoctorCommand(
       int DoctorId,
       string Fullname,
       DateTime Birthdate,
       string Phone,
       string? NationalNo,
       string Address,
       string? Specialization
   ) : IRequest<Result<Updated>>;
