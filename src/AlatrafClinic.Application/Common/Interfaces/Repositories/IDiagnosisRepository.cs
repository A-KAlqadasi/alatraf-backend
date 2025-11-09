using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.DiagnosisPrograms;
using AlatrafClinic.Domain.Diagnosises.Enums;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IDiagnosisRepository : IGenericRepository<Diagnosis, int>
{
    Task<IQueryable<Diagnosis>> GetDiagnosesQueryAsync(CancellationToken ct = default);
    Task<DiagnosisProgram?> GetDiagnosisProgramByIdAsync(int id, CancellationToken ct = default);  
}