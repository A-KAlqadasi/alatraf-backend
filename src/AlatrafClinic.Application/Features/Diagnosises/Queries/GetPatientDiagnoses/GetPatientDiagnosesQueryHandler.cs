
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Domain.Common.Results;


using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Diagnosises.Queries.GetPatientDiagnoses;

public class GetPatientDiagnosesQueryHandler : IRequestHandler<GetPatientDiagnosesQuery, Result<List<DiagnosisDto>>>
{
    private readonly ILogger<GetPatientDiagnosesQueryHandler> _logger;
    private readonly IUnitOfWork _uow;

    public GetPatientDiagnosesQueryHandler(ILogger<GetPatientDiagnosesQueryHandler> logger, IUnitOfWork uow)
    {
        _logger = logger;
        _uow = uow;
    }
    public async Task<Result<List<DiagnosisDto>>> Handle(GetPatientDiagnosesQuery query, CancellationToken ct)
    {
        var patientDiagnosis = await _uow.Diagnosises.GetPatientDiagnosesAsync(query.patientId, ct);

        if (patientDiagnosis is null || patientDiagnosis.Count == 0)
        {
            _logger.LogWarning("No diagnoses found for Patient with Id {PatientId}.", query.patientId);
            return Error.NotFound(code: "Diagnoses.NotFound",
            description: $"No diagnoses found for Patient with Id {query.patientId}.");
        }

        return patientDiagnosis.ToDtos();
    }
}