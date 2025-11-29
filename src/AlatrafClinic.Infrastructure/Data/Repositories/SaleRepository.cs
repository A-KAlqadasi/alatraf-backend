
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Sales;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class SaleRepository : GenericRepository<Sale, int>, ISaleRepository
{
    public SaleRepository(ApplicationDbContext dbContext): base(dbContext)
    {
        
    }
    public async Task<Sale?> GetByDiagnosisIdAsync(int diagnosisId, CancellationToken ct)
    {
        return await _dbContext.Sales
            .SingleOrDefaultAsync(s => s.DiagnosisId == diagnosisId, ct);
    }
}