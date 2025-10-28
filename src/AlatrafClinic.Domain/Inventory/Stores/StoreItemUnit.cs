using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;

namespace AlatrafClinic.Domain.Inventory.Stores;

public class StoreItemUnit : AuditableEntity<int>
{
    public int StoreId { get; private set; }
    public Store Store { get; private set; } = default!;
    public int ItemUnitId { get; private set; }
    public ItemUnit ItemUnit { get; private set; } = default!;

    public decimal Quantity { get; private set; }

    private StoreItemUnit() { }

    private StoreItemUnit(Store store, ItemUnit itemUnit, decimal quantity)
    {
        Store = store;
        StoreId = store.Id;
        ItemUnit = itemUnit;
        ItemUnitId = itemUnit.Id;
        Quantity = quantity;
    }

    public static Result<StoreItemUnit> Create(Store store, ItemUnit itemUnit, decimal quantity)
    {
        if (store is null)
            return StoreItemUnitErrors.StoreIsRequired;
        if (itemUnit is null)
            return StoreItemUnitErrors.ItemIsRequired;
        if (quantity < 0)
            return StoreItemUnitErrors.InvalidQuantity;

        return new StoreItemUnit(store, itemUnit, quantity);
    }

    public Result<Updated> Increase(decimal quantity)
    {
        if (quantity <= 0)
            return StoreItemUnitErrors.InvalidQuantity;

        Quantity += quantity;
        return Result.Updated;
    }

    public Result<Updated> Decrease(decimal quantity)
    {
        if (quantity <= 0)
            return StoreItemUnitErrors.InvalidQuantity;

        if (Quantity < quantity)
            return StoreItemUnitErrors.QuantityNotEnough;

        Quantity -= quantity;
        return Result.Updated;
    }
}