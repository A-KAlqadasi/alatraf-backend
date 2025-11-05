using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Diagnosises.Queries.GetDiagnoses;

public sealed record GetDiagnosesQuery : IRequest<Result<List<DiagnosisDto>>>
{
}