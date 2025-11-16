using AlatrafClinic.Domain.RepairCards;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IRepairCardRepository : IGenericRepository<RepairCard, int>
{
    Task<IQueryable<RepairCard>> GetRepairCardsQueryAsync(CancellationToken ct = default);
}