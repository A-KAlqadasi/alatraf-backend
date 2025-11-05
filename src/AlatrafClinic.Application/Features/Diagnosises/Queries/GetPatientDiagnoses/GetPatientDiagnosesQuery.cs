using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Diagnosises.Queries.GetPatientDiagnoses;

public sealed record GetPatientDiagnosesQuery(int patientId) : IRequest<Result<List<DiagnosisDto>>>;