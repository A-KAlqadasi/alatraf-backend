using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.Enums;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IDiagnosisRepository : IGenericRepository<Diagnosis, int>
{
    Task<IReadOnlyList<Diagnosis>?> GetPatientDiagnosesAsync(int patientId, CancellationToken ct = default);
    Task<IReadOnlyList<Diagnosis>?> GetPatientDiagnosesTypeAsync(int patientId, DiagnosisType type, CancellationToken ct = default);
    
}