using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Diagnosises.Queries.GetDiagnosisById;

public sealed record GetDiagnosisByIdQuery(int DiagnosisId)
    : ICachedQuery<Result<DiagnosisDto>>
{
    public string CacheKey => $"diagnosis:{DiagnosisId}";
    public string[] Tags => ["diagnosis"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(15);
}