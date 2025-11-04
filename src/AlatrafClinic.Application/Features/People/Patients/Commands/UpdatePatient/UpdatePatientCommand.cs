using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Enums;
using MediatR;

namespace AlatrafClinic.Application.Features.People.Patients.Commands.UpdatePatient;

public sealed record UpdatePatientCommand(
    int PatientId,
    PatientType PatientType
) : IRequest<Result<Updated>>;
