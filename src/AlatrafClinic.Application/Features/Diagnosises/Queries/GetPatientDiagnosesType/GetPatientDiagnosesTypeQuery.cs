using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises.Enums;

using MediatR;

namespace AlatrafClinic.Application.Features.Diagnosises.Queries.GetPatientDiagnosesType;

public sealed record GetPatientDiagnosesTypeQuery(int patientId, DiagnosisType type) : IRequest<Result<List<DiagnosisDto>>>;