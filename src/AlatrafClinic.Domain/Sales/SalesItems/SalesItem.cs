using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Sales.SalesItems;

public class SalesItem : AuditableEntity<int>
{
    public int SaleId { get; private set; }
    public Sale Sale { get; private set; } = default!;

    public int ItemId { get; private set; }
    // public Item Item { get; private set; } = default!;

    public int UnitId { get; private set; }
    // public Unit Unit { get; private set; } = default!;

    public decimal Quantity { get; private set; }
    public decimal Price { get; private set; }

    public decimal Total => Quantity * Price;

    private SalesItem() { }

    private SalesItem(int itemId, int unitId, decimal quantity, decimal price)
    {
        ItemId = itemId;
        UnitId = unitId;
        Quantity = quantity;
        Price = price;
    }

    public static Result<SalesItem> Create(int itemId, int unitId, decimal quantity, decimal price)
    {
        if (itemId <= 0)
            return SalesItemErrors.ItemRequired;

        if (unitId <= 0)
            return SalesItemErrors.UnitRequired;

        if (quantity <= 0)
            return SalesItemErrors.InvalidQuantity;

        if (price < 0)
            return SalesItemErrors.InvalidPrice;

        return new SalesItem(itemId, unitId, quantity, price);
    }

    public Result<Updated> Update(decimal quantity, decimal price)
    {
        if (quantity <= 0)
            return SalesItemErrors.InvalidQuantity;

        if (price < 0)
            return SalesItemErrors.InvalidPrice;

        Quantity = quantity;
        Price = price;

        return Result.Updated;
    }
}
