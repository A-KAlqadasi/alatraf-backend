
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Sales;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class SaleRepository : GenericRepository<Sale, int>, ISaleRepository
{
    public SaleRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {

    }
    public async Task<Sale?> GetByDiagnosisIdAsync(int diagnosisId, CancellationToken ct)
    {
        return await dbContext.Sales
            .SingleOrDefaultAsync(s => s.DiagnosisId == diagnosisId, ct);
    }
    public new async Task<Sale?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await dbContext.Sales
            .Include(s => s.SaleItems)
            .Include(s => s.Diagnosis)
            .SingleOrDefaultAsync(s => s.Id == id, ct);
    }
}