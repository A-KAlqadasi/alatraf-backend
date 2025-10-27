using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;

namespace AlatrafClinic.Domain.RepairCards.Orders;

public class OrderItem : AuditableEntity<int>
{
    public int? OrderId { get; set; }
    public Order? Order { get; set; }
    public int? ItemId { get; set; }
    public Item? Item { get; set; }
    public int ItemUnitId { get; set; }
    public ItemUnit? ItemUnit { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal TotalPrice => Quantity * Price;

    private OrderItem() { }
    private OrderItem(int itemUnitId, decimal quantity, decimal price)
    {
        ItemUnitId = itemUnitId;
        Quantity = quantity;
        Price = price;
    }
    public static Result<OrderItem> Create(int itemUnitId, decimal quantity, decimal price)
    {
        if (itemUnitId <= 0)
        {
            return OrderItemErrors.ItemUnitIdInvalid;
        }
        if (quantity <= 0)
        {
            return OrderItemErrors.QuantityInvalid;
        }
        if (price <= 0)
        {
            return OrderItemErrors.PriceInvalid;
        }

        return new OrderItem(itemUnitId, quantity, price);
    }

    public Result<Updated> Update(int itemUnitId, decimal quantity, decimal price)
    {
        if (itemUnitId <= 0)
        {
            return OrderItemErrors.ItemUnitIdInvalid;
        }
        if (quantity <= 0)
        {
            return OrderItemErrors.QuantityInvalid;
        }
        if (price <= 0)
        {
            return OrderItemErrors.PriceInvalid;
        }
        ItemUnitId = itemUnitId;
        Quantity = quantity;
        Price = price;

        return Result.Updated;
    }
}