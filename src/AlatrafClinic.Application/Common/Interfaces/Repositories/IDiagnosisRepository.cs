using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.DiagnosisIndustrialParts;
using AlatrafClinic.Domain.Diagnosises.DiagnosisPrograms;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IDiagnosisRepository : IGenericRepository<Diagnosis, int>
{
    Task<DiagnosisProgram?> GetDiagnosisProgramByIdAsync(int id, CancellationToken ct = default);
    Task<DiagnosisIndustrialPart?> GetDiagnosisIndustrialPartByIdAsync(int id, CancellationToken ct = default);
    
}