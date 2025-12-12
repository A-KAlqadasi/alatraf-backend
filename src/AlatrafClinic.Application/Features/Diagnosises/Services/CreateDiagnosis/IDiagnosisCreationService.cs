using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.Enums;

namespace AlatrafClinic.Application.Features.Diagnosises.Services.CreateDiagnosis;

public interface IDiagnosisCreationService
{
    Task<Result<Diagnosis>> CreateAsync(
        int ticketId,
        string diagnosisText,
        DateOnly injuryDate,
        List<int> injuryReasons,
        List<int> injurySides,
        List<int> injuryTypes,
        DiagnosisType diagnosisType,
        CancellationToken ct);
}
