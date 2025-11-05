using Microsoft.Extensions.Logging;

using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;


namespace AlatrafClinic.Application.Features.Diagnosises.Queries.GetPatientDiagnosesType;

public class GetPatientDiagnosesTypeQueryHandler : IRequestHandler<GetPatientDiagnosesTypeQuery, Result<List<DiagnosisDto>>>
{
    private readonly ILogger<GetPatientDiagnosesTypeQueryHandler> _logger;
    private readonly IUnitOfWork _uow;

    public GetPatientDiagnosesTypeQueryHandler(ILogger<GetPatientDiagnosesTypeQueryHandler> logger, IUnitOfWork uow)
    {
        _logger = logger;
        _uow = uow;
    }
    public async Task<Result<List<DiagnosisDto>>> Handle(GetPatientDiagnosesTypeQuery query, CancellationToken ct)
    {
        var patientDiagnosis = await _uow.Diagnosises.GetPatientDiagnosesTypeAsync(query.patientId, query.type, ct);

        if (patientDiagnosis is null || patientDiagnosis.Count == 0)
        {
            _logger.LogWarning("No diagnoses found for Patient with Id {PatientId}.", query.patientId);
            return Error.NotFound(code: "Diagnoses.NotFound",
            description: $"No diagnoses found for Patient with Id {query.patientId}.");
        }

        return patientDiagnosis.ToDtos();
    }
}