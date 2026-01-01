using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Diagnosises.Queries.GetDiagnosisById;

public sealed record GetDiagnosisByIdQuery(int DiagnosisId)
    : IRequest<Result<DiagnosisDto>>;