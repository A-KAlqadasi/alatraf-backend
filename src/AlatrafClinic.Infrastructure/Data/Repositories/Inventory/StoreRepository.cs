using AlatrafClinic.Application.Common.Interfaces.Repositories.Inventory;
using AlatrafClinic.Application.Features.Inventory.Stores.Dtos;
using AlatrafClinic.Domain.Inventory.Stores;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories.Inventory;

public class StoreRepository : GenericRepository<Store, int>, IStoreRepository
{
    public StoreRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Store?> GetByIdWithItemUnitsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Stores
            .Include(s => s.StoreItemUnits)
                .ThenInclude(siu => siu.ItemUnit)
            .AsQueryable()
            .SingleOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<StoreItemUnitDto>> GetItemUnitsAsync(int storeId, CancellationToken cancellationToken = default)
    {
        return await dbContext.StoreItemUnits
            .AsNoTracking()
            .Where(siu => siu.StoreId == storeId)
            .Select(siu => new StoreItemUnitDto
            {
                StoreItemUnitId = siu.Id,
                StoreId = siu.StoreId,
                ItemId = siu.ItemUnit.ItemId,
                ItemName = siu.ItemUnit.Item.Name,
                UnitId = siu.ItemUnit.UnitId,
                UnitName = siu.ItemUnit.Unit.Name,
                Quantity = siu.Quantity
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasAssociationsAsync(int StoreId, CancellationToken cancellationToken = default)
    {
        // check commonly associated aggregates: PurchaseInvoices, ExchangeOrders, Sales
        var hasPurchase = await dbContext.PurchaseInvoices.AnyAsync(pi => pi.StoreId == StoreId, cancellationToken);
        if (hasPurchase) return true;

        var hasExchange = await dbContext.ExchangeOrders.AnyAsync(e => e.StoreId == StoreId, cancellationToken);
        if (hasExchange) return true;

        // check sale items that reference store item units belonging to the store
        var hasSaleItems = await dbContext.SaleItems.AnyAsync(si => dbContext.StoreItemUnits.Any(siu => siu.Id == si.StoreItemUnitId && siu.StoreId == StoreId), cancellationToken);
        if (hasSaleItems) return true;

        return false;
    }
}
