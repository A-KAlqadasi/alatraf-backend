using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;
using AlatrafClinic.Domain.Inventory.Stores;

namespace AlatrafClinic.Domain.RepairCards.Orders;
public class OrderItem : AuditableEntity<int>
{
    public int OrderId { get; private set; }
    public Order Order { get; set; } = default!;

    public int StoreItemUnitId { get; private set; }
    public StoreItemUnit StoreItemUnit { get; set; } = default!;

    public decimal Quantity { get; private set; }
    public decimal Price { get; private set; }
    public decimal Total => Quantity * Price;

    private OrderItem() { }

    private OrderItem(int orderId, int storeItemUnitId, decimal quantity, decimal price)
    {
        OrderId = orderId;
        StoreItemUnitId = storeItemUnitId;
        Quantity = quantity;
        Price = price;
    }

    public static Result<OrderItem> Create(int orderId, int storeItemUnitId, decimal quantity, decimal price)
    {
        if (orderId <= 0) return OrderItemErrors.OrderIdIsRequired;
        if (storeItemUnitId <= 0) return OrderItemErrors.StoreItemRequired;
        if (quantity <= 0) return OrderItemErrors.InvalidQuantity;
        if (price < 0) return OrderItemErrors.InvalidPrice;

        return new OrderItem(orderId, storeItemUnitId, quantity, price);
    }

    internal Result<Updated> Update(int orderId, int storeItemUnitId, decimal quantity, decimal price)
    {
        if (orderId <= 0) return OrderItemErrors.OrderIdIsRequired;
        if (storeItemUnitId <= 0) return OrderItemErrors.StoreItemRequired;
        if (quantity <= 0) return OrderItemErrors.InvalidQuantity;
        if (price < 0) return OrderItemErrors.InvalidPrice;

        OrderId = orderId;
        StoreItemUnitId = storeItemUnitId;
        Quantity = quantity;
        Price = price;

        return Result.Updated;
    }
}