using AlatrafClinic.Domain.Sales;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface ISaleRepository : IGenericRepository<Sale, int>
{
    Task<Sale?> GetByDiagnosisIdAsync(int diagnosisId, CancellationToken ct);
    Task<IQueryable<Sale>> GetSalesQueryAsync();
}
