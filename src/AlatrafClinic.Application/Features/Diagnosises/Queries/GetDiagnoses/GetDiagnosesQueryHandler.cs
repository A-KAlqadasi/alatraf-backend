using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;

using MediatR;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Diagnosises.Queries.GetDiagnoses;

public class GetDiagnosesQueryHandler : IRequestHandler<GetDiagnosesQuery, Result<List<DiagnosisDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetDiagnosesQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }
    public async Task<Result<List<DiagnosisDto>>> Handle(GetDiagnosesQuery query, CancellationToken ct)
    {
        IReadOnlyList<Diagnosis> diagnoses = await _uow.Diagnosises.GetAllAsync(ct);

        return diagnoses.ToDtos();
    }
}