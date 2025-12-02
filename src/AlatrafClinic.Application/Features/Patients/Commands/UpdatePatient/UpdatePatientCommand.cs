using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Enums;

using MediatR;

namespace AlatrafClinic.Application.Features.Patients.Commands.UpdatePatient;


public sealed record UpdatePatientCommand(
    int PatientId,
    string Fullname,
    DateTime Birthdate,
    string Phone,
    string? NationalNo,
    string Address,
    bool Gender,
    PatientType PatientType
) : IRequest<Result<Updated>>;