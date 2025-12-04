using AlatrafClinic.Domain.Inventory.ExchangeOrders;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories.Inventory;

public interface IExchangeOrderRepository : IGenericRepository<ExchangeOrder, int>
{
    // Projection for advanced read scenarios
    Task<IQueryable<ExchangeOrder>> GetExchangeOrdersQueryAsync(CancellationToken ct = default);
}
