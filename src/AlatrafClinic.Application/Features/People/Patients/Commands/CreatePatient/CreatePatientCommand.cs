using AlatrafClinic.Application.Features.People.Patients.Dtos;
using AlatrafClinic.Application.Features.People.Persons.Services;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Enums;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Patients.Commands.CreatePatient;

public sealed record CreatePatientCommand(
 PersonInput Person,
     PatientType PatientType
// string? AutoRegistrationNumber
) : IRequest<Result<PatientDto>>;
