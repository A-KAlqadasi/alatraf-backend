using AlatrafClinic.Application.Features.Inventory.Stores.Dtos;
using AlatrafClinic.Domain.Inventory.Stores;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Mappers;

public static class StoreMapper
{
    public static StoreDto ToDto(this Store store)
    {
        return new StoreDto
        {
            StoreId = store.Id,
            Name = store.Name,
            TotalQuantity = store.GetTotalQuantity()
        };
    }

    public static List<StoreDto> ToDtos(this IEnumerable<Store> stores)
    {
        return stores.Select(s => s.ToDto()).ToList();
    }
}
