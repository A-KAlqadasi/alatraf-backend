using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Stores;

namespace AlatrafClinic.Domain.Inventory.ExchangeOrders;

public class ExchangeOrderItem : AuditableEntity<int>
{
    public int ExchangeOrderId { get; private set; }
    public ExchangeOrder ExchangeOrder { get;  set; } = default!;

    public int StoreItemUnitId { get; private set; }
    public StoreItemUnit StoreItemUnit { get;  set; } = default!;

    public decimal Quantity { get; private set; }

    private ExchangeOrderItem() { }

    private ExchangeOrderItem(int exchangeOrderId, int storeItemUnitId, decimal quantity)
    {
        ExchangeOrderId = exchangeOrderId;
        StoreItemUnitId = storeItemUnitId;
        Quantity = quantity;
    }

    public static Result<ExchangeOrderItem> Create(int exchangeOrderId, int storeItemUnitId, decimal quantity)
    {
        if (exchangeOrderId <= 0)
            return ExchangeOrderErrors.ExchangeOrderRequired;

        if (storeItemUnitId <= 0)
            return ExchangeOrderErrors.InvalidItem;
        if (quantity <= 0)
            return ExchangeOrderErrors.InvalidQuantity;

        return new ExchangeOrderItem(exchangeOrderId, storeItemUnitId, quantity);
    }
    public Result<Updated> Update(int exchangeOrderId, int storeItemUnitId, decimal quantity)
    {
        if (exchangeOrderId <= 0) return ExchangeOrderErrors.ExchangeOrderRequired;
        if (storeItemUnitId <= 0) return ExchangeOrderErrors.InvalidItem;
        if (quantity <= 0) return ExchangeOrderErrors.InvalidQuantity;
        ExchangeOrderId = exchangeOrderId;
        StoreItemUnitId = storeItemUnitId;
        Quantity = quantity;
        return Result.Updated;
    }
}