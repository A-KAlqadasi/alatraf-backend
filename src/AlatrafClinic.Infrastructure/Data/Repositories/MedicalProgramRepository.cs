using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.TherapyCards.MedicalPrograms;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class MedicalProgramRepository : GenericRepository<MedicalProgram, int>, IMedicalProgramRepository
{
    public MedicalProgramRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> IsExistsByName(string name, CancellationToken ct = default)
    {
        return await dbContext.MedicalPrograms
            .AnyAsync(mp => mp.Name == name, ct);
    }
}