
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.DiagnosisIndustrialParts;
using AlatrafClinic.Domain.Diagnosises.DiagnosisPrograms;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class DiagnosisRepository : GenericRepository<Diagnosis, int>, IDiagnosisRepository
{
    public DiagnosisRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<DiagnosisIndustrialPart?> GetDiagnosisIndustrialPartByIdAsync(int id, CancellationToken ct = default)
    {
        return await dbContext.DiagnosisIndustrialParts.FirstOrDefaultAsync(dip=> dip.Id == id);
    }

    public async Task<DiagnosisProgram?> GetDiagnosisProgramByIdAsync(int id, CancellationToken ct = default)
    {
        return await dbContext.DiagnosisPrograms.FirstOrDefaultAsync(dp => dp.Id == id);
    }
}