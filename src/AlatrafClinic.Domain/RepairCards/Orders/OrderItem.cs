using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;
using AlatrafClinic.Domain.Inventory.Stores;

namespace AlatrafClinic.Domain.RepairCards.Orders;
public class OrderItem : AuditableEntity<int>
{
    public int OrderId { get; private set; }
    public Order Order { get; private set; } = default!;

    public int StoreItemUnitId { get; private set; }
    public StoreItemUnit StoreItemUnit { get; private set; } = default!;

    public decimal Quantity { get; private set; }
    public decimal Price { get; private set; }
    public decimal Total => Quantity * Price;

    private OrderItem() { }

    private OrderItem(StoreItemUnit storeItemUnit, decimal quantity, decimal price)
    {
        StoreItemUnit = storeItemUnit;
        StoreItemUnitId = storeItemUnit.Id;
        Quantity = quantity;
        Price = price;
    }

    public static Result<OrderItem> Create(StoreItemUnit storeItemUnit, decimal quantity, decimal price)
    {
        if (storeItemUnit == null) return OrderItemErrors.StoreItemRequired;
        if (quantity <= 0)         return OrderItemErrors.InvalidQuantity;
        if (price < 0)             return OrderItemErrors.InvalidPrice;

        return new OrderItem(storeItemUnit, quantity, price);
    }

    internal Result<Updated> Update(StoreItemUnit storeItemUnit, decimal quantity, decimal price)
    {
        if (storeItemUnit == null) return OrderItemErrors.StoreItemRequired;
        if (quantity <= 0)         return OrderItemErrors.InvalidQuantity;
        if (price < 0)             return OrderItemErrors.InvalidPrice;

        StoreItemUnit = storeItemUnit;
        StoreItemUnitId = storeItemUnit.Id;
        Quantity = quantity;
        Price = price;

        return Result.Updated;
    }
}