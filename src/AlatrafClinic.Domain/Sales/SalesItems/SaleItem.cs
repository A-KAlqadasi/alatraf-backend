using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Stores;

namespace AlatrafClinic.Domain.Sales.SalesItems;

public class SaleItem : AuditableEntity<int>
{
   public int SaleId { get; private set; }
    public Sale Sale { get; private set; } = default!;

    // We sell from a specific store => point to StoreItemUnit (not raw Item/Unit)
    public int StoreItemUnitId { get; private set; }
    public StoreItemUnit StoreItemUnit { get; private set; } = default!;

    public decimal Quantity { get; private set; }
    public decimal Price { get; private set; }         // can be zero if fully discounted
    public decimal Total => Quantity * Price;

    private SaleItem() { }

    private SaleItem(StoreItemUnit storeItemUnit, decimal quantity, decimal price)
    {
        StoreItemUnit = storeItemUnit;
        StoreItemUnitId = storeItemUnit.Id;
        Quantity = quantity;
        Price = price;
    }

    public static Result<SaleItem> Create(StoreItemUnit storeItemUnit, decimal quantity, decimal price)
    {
        if (storeItemUnit is null)  return SaleItemErrors.InvalidItem;
        if (quantity <= 0)          return SaleItemErrors.InvalidQuantity;
        if (price < 0)              return SaleItemErrors.InvalidPrice;

        return new SaleItem(storeItemUnit, quantity, price);
    }

    internal Result<Updated> Update(StoreItemUnit storeItemUnit, decimal quantity, decimal price)
    {
        if (storeItemUnit is null)  return SaleItemErrors.InvalidItem;
        if (quantity <= 0)          return SaleItemErrors.InvalidQuantity;
        if (price < 0)              return SaleItemErrors.InvalidPrice;

        StoreItemUnit = storeItemUnit;
        StoreItemUnitId = storeItemUnit.Id;
        Quantity = quantity;
        Price = price;

        return Result.Updated;
    }

    internal void IncreaseQuantity(decimal by) => Quantity += by; // caller validates
}
