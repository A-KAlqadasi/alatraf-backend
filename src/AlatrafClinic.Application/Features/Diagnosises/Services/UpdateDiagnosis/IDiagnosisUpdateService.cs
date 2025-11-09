using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.Enums;

namespace AlatrafClinic.Application.Features.Diagnosises.Services.UpdateDiagnosis;

public interface IDiagnosisUpdateService
{
    Task<Result<Diagnosis>> UpdateAsync(
        int diagnosisId,
        int ticketId,
        string diagnosisText,
        DateTime injuryDate,
        List<int> injuryReasons,
        List<int> injurySides,
        List<int> injuryTypes,
        int patientId,
        DiagnosisType diagnosisType,
        CancellationToken ct);
}