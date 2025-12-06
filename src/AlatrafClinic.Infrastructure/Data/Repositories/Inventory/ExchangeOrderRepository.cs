using AlatrafClinic.Application.Common.Interfaces.Repositories.Inventory;
using AlatrafClinic.Domain.Inventory.ExchangeOrders;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories.Inventory;

public class ExchangeOrderRepository : GenericRepository<ExchangeOrder, int>, IExchangeOrderRepository
{
    public ExchangeOrderRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }

    public Task<IQueryable<ExchangeOrder>> GetExchangeOrdersQueryAsync(CancellationToken ct = default)
    {
        // return queryable for advanced scenarios - do not execute here
        IQueryable<ExchangeOrder> query = dbContext.ExchangeOrders.AsQueryable();
        return Task.FromResult(query);
    }
}
