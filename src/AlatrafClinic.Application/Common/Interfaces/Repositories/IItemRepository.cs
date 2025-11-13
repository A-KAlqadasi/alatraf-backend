using AlatrafClinic.Domain.Inventory.Items;
using AlatrafClinic.Domain.Sales;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IItemRepository : IGenericRepository<Item, int>
{
    Task<ItemUnit?> GetByIdAndUnitIdAsync(int id, int unitId, CancellationToken ct);
}