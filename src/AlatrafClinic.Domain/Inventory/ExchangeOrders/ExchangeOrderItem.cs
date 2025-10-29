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

    private ExchangeOrderItem(StoreItemUnit storeItemUnit, decimal quantity)
    {
        StoreItemUnit = storeItemUnit;
        StoreItemUnitId = storeItemUnit.Id;
        Quantity = quantity;
    }

    public static Result<ExchangeOrderItem> Create(StoreItemUnit storeItemUnit, decimal quantity)
    {
        if (storeItemUnit == null)
            return ExchangeOrderErrors.InvalidItem;
        if (quantity <= 0)
            return ExchangeOrderErrors.InvalidQuantity;

        return new ExchangeOrderItem(storeItemUnit, quantity);
    }
}