
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.DiagnosisIndustrialParts;
using AlatrafClinic.Domain.Diagnosises.DiagnosisPrograms;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class DiagnosisRepository : GenericRepository<Diagnosis, int>, IDiagnosisRepository
{
    private readonly ApplicationDbContext _dbContext;

    public DiagnosisRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DiagnosisIndustrialPart?> GetDiagnosisIndustrialPartByIdAsync(int id, CancellationToken ct = default)
    {
        return await _dbContext.DiagnosisIndustrialParts.FirstOrDefaultAsync(dip=> dip.Id == id);
    }

    public async Task<DiagnosisProgram?> GetDiagnosisProgramByIdAsync(int id, CancellationToken ct = default)
    {
        return await _dbContext.DiagnosisPrograms.FirstOrDefaultAsync(dp => dp.Id == id);
    }
}