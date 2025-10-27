using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Sales.SalesItems;

public class SalesItem : AuditableEntity<int>
{
    public int SaleId { get; private set; }
     public Sales? Sales { get; set; }
    public int ItemId { get; private set; }
    // public Items? Item { get; set; }
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    public string Unit { get; private set; } = default!;

    private SalesItem(int salesId, int itemId, int quantity, decimal price, string unit)
    {
        SaleId = salesId;
        ItemId = itemId;
        Quantity = quantity;
        Price = price;
        Unit = unit;
    }

    public static Result<SalesItem> Create(int salesId, int itemId, int quantity, decimal price, string unit)
    {
        if (quantity <= 0) return SalesItemErrors.QuantityInvalid;
        if (price    <= 0) return SalesItemErrors.PriceInvalid;
        if (price    <= 0) return SalesItemErrors.PriceInvalid;
        if (string.IsNullOrWhiteSpace(unit)) return SalesItemErrors.UnitRequired;

        return new SalesItem(salesId, itemId, quantity, price, unit);
    }

    public Result<Updated> Update(int quantity, decimal price, string unit)
    {
        if (quantity <= 0) return SalesItemErrors.QuantityInvalid;
        if (price    <= 0) return SalesItemErrors.PriceInvalid;
        if (string.IsNullOrWhiteSpace(unit)) return SalesItemErrors.UnitRequired;

        Quantity = quantity;
        Price = price;
        Unit = unit;

        return Result.Updated;
    }

    public decimal CalculateTotal() => Quantity * Price;
}  

