using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Stores;

namespace AlatrafClinic.Domain.Inventory.ExchangeOrders;

public class ExchangeOrderItem : AuditableEntity<int>
{
    public int ExchangeOrderId { get; private set; }
    public ExchangeOrder ExchangeOrder { get; private set; } = default!;

    public int StoreItemUnitId { get; private set; }
    public StoreItemUnit StoreItemUnit { get; private set; } = default!;

    public decimal Quantity { get; private set; }

    private ExchangeOrderItem() { }

    private ExchangeOrderItem(int storeItemUnitId, decimal quantity)
    {
        StoreItemUnitId = storeItemUnitId;
        Quantity = quantity;
    }

    // ============================================================
    // FACTORY
    // Parent ID is NOT required. Aggregate root sets the relation.
    // ============================================================
    public static Result<ExchangeOrderItem> Create(int storeItemUnitId, decimal quantity)
    {
        if (storeItemUnitId <= 0)
            return ExchangeOrderErrors.InvalidItem;

        if (quantity <= 0)
            return ExchangeOrderErrors.InvalidQuantity;

        return new ExchangeOrderItem(storeItemUnitId, quantity);
    }

    // ============================================================
    // UPDATE (only item-level invariants)
    // Parent ID set by EF or aggregate root
    // ============================================================
    public Result<Updated> Update(int storeItemUnitId, decimal quantity)
    {
        if (storeItemUnitId <= 0)
            return ExchangeOrderErrors.InvalidItem;

        if (quantity <= 0)
            return ExchangeOrderErrors.InvalidQuantity;

        StoreItemUnitId = storeItemUnitId;
        Quantity = quantity;

        return Result.Updated;
    }
}
