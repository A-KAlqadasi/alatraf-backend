using AlatrafClinic.Application.Features.Patients.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Enums;

using MediatR;

namespace AlatrafClinic.Application.Features.Patients.Commands.CreatePatient;

public sealed record CreatePatientCommand(
    string Fullname,
    DateTime Birthdate,
    string Phone,
    string? NationalNo,
    string Address,
    bool Gender,
    PatientType PatientType
) : IRequest<Result<PatientDto>>;
