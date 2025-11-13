using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Domain.Inventory.Items;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories.Inventory;

public interface IItemRepository : IGenericRepository<Item, int>
{

    // Task<ItemUnit?> GetByIdAndUnitIdAsync(int id, int unitId, CancellationToken ct);
    Task<IEnumerable<Item>> GetAllWithUnitsAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Item>> GetInactiveAsync(CancellationToken cancellationToken);
    Task<Item?> GetByIdWithUnitsAsync(int id, CancellationToken cancellationToken);
    Task<PagedResult<Item>> SearchAsync(ItemSearchSpec spec, CancellationToken cancellationToken);



}
