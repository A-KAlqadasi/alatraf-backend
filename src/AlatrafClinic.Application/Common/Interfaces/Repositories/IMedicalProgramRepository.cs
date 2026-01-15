using AlatrafClinic.Domain.MedicalPrograms;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IMedicalProgramRepository : IGenericRepository<MedicalProgram, int>
{
    Task<bool> IsExistsByName(string name, CancellationToken ct = default);
}