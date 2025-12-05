using AlatrafClinic.Application.Features.Inventory.Stores.Dtos;
using AlatrafClinic.Domain.Inventory.Stores;
namespace AlatrafClinic.Application.Common.Interfaces.Repositories.Inventory;

public interface IStoreRepository : IGenericRepository<Store, int>
{
    Task<Store?> GetByIdWithItemUnitsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<StoreItemUnitDto>> GetItemUnitsAsync(int storeId, CancellationToken cancellationToken = default);
}
