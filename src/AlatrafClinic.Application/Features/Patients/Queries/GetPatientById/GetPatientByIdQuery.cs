using AlatrafClinic.Application.Features.Patients.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Patients.Queries.GetPatientById;

public sealed record GetPatientByIdQuery(int PatientId) : IRequest<Result<PatientDto>>;
