using AlatrafClinic.Application.Features.People.Patients.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Enums;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Patients.Commands.CreatePatient;

public sealed record CreatePatientCommand(
    int PersonId,
    PatientType PatientType
    // string? AutoRegistrationNumber
) : IRequest<Result<PatientDto>>;
