using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.RepairCards.IndustrialParts;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class IndustrialPartRepository : GenericRepository<IndustrialPart, int>, IIndustrialPartRepository
{
    public IndustrialPartRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IndustrialPartUnit?> GetByIdAndUnitId(int Id, int unitId, CancellationToken ct)
    {
        return await _dbContext.IndustrialPartUnits
            .FirstOrDefaultAsync(ipu => ipu.IndustrialPartId == Id && ipu.UnitId == unitId, ct);
    }

    public async Task<bool> IsExistsByName(string name, CancellationToken ct)
    {
        return await _dbContext.IndustrialParts
            .AnyAsync(ip => ip.Name == name, ct);
    }
}