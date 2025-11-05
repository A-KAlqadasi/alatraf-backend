using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Diagnosises.Queries.GetDiagnosisById;

public class GetDiagnosisByIdQueryHandler : IRequestHandler<GetDiagnosisByIdQuery, Result<DiagnosisDto>>
{
    private readonly ILogger<GetDiagnosisByIdQueryHandler> _logger;
    private readonly IUnitOfWork _uow;

    public GetDiagnosisByIdQueryHandler(ILogger<GetDiagnosisByIdQueryHandler> logger, IUnitOfWork uow)
    {
        _logger = logger;
        _uow = uow;
    }
    public async Task<Result<DiagnosisDto>> Handle(GetDiagnosisByIdQuery query, CancellationToken ct)
    {
        Diagnosis? diagnosis = await _uow.Diagnosises.GetByIdAsync(query.diagnosisId, ct);
        
        if (diagnosis is null)
        {
            _logger.LogWarning("Diagnosis with Id {DiagnosisId} not found.", query.diagnosisId);
            return Error.NotFound(code: "Diagnosis.NotFound",
            description: $"Diagnosis with Id {query.diagnosisId} not found.");
        }

        return diagnosis.ToDto();
    }
}