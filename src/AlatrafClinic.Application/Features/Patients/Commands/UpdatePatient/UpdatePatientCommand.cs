using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Enums;

using MediatR;

namespace AlatrafClinic.Application.Features.Patients.Commands.UpdatePatient;


public sealed record UpdatePatientCommand(
    int PatientId,
    string Fullname,
    DateOnly Birthdate,
    string Phone,
    string? NationalNo,
    int AddressId,
    bool Gender,
    PatientType PatientType
) : IRequest<Result<Updated>>;