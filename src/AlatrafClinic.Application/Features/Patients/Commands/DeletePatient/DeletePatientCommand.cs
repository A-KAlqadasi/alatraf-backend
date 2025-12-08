using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Patients.Commands.DeletePatient;

public sealed record class DeletePatientCommand(int PatientId) : IRequest<Result<Deleted>>;