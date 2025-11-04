using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;
using AlatrafClinic.Domain.Inventory.Stores;

namespace AlatrafClinic.Domain.RepairCards.Orders;
public class OrderItem : AuditableEntity<int>
{
    public int OrderId { get; private set; }
    public Order Order { get; set; } = default!;

    public int ItemUnitId { get; private set; }
    public ItemUnit ItemUnit { get; set; } = default!;

    public decimal Quantity { get; private set; }
    public decimal Price { get; private set; }
    public decimal Total => Quantity * Price;

    private OrderItem() { }

    private OrderItem(int orderId, ItemUnit itemUnit, decimal quantity)
    {
        OrderId = orderId;
        ItemUnit = itemUnit;
        ItemUnitId = itemUnit.Id;
        Quantity = quantity;
        Price = itemUnit.Price;
    }

    public static Result<OrderItem> Create(int orderId, ItemUnit itemUnit, decimal quantity)
    {
        if (itemUnit is null) return OrderItemErrors.ItemUnitRequired;
        if (quantity <= 0) return OrderItemErrors.InvalidQuantity;


        return new OrderItem(orderId, itemUnit, quantity);
    }

    internal Result<Updated> Update(int orderId, ItemUnit itemUnit, decimal quantity)
    {
        if (orderId <= 0) return OrderItemErrors.OrderIdIsRequired;
        if (itemUnit is null) return OrderItemErrors.ItemUnitRequired;
        if (quantity <= 0) return OrderItemErrors.InvalidQuantity;

        OrderId = orderId;
        ItemUnit = itemUnit;
        ItemUnitId = itemUnit.Id;
        Quantity = quantity;
        Price = itemUnit.Price;

        return Result.Updated;
    }
}