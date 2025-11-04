using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Stores;

namespace AlatrafClinic.Domain.Sales.SalesItems;

public class SaleItem : AuditableEntity<int>
{
    public int SaleId { get; private set; }
    public Sale Sale { get; set; } = default!;

    public int StoreItemUnitId { get; private set; }
    public StoreItemUnit StoreItemUnit { get; set; } = default!;

    public decimal Quantity { get; private set; }
    public decimal Price { get; private set; }
    public decimal Total => Quantity * Price;

    private SaleItem() { }

    private SaleItem(int saleId, int storeItemUnitId, decimal quantity, decimal price)
    {
        SaleId = saleId;
        StoreItemUnitId = storeItemUnitId;
        Quantity = quantity;
        Price = price;
    }

    public static Result<SaleItem> Create(int saleId, int storeItemUnitId, decimal quantity, decimal price)
    {
        if (storeItemUnitId <= 0)  return SaleItemErrors.InvalidItem;
        if (quantity <= 0)          return SaleItemErrors.InvalidQuantity;
        if (price < 0)              return SaleItemErrors.InvalidPrice;

        return new SaleItem(saleId, storeItemUnitId, quantity, price);
    }

    internal Result<Updated> Update(int saleId, int storeItemUnitId, decimal quantity, decimal price)
    {
        if (saleId <= 0) return SaleItemErrors.InvalidSaleId;
        if (StoreItemUnitId <= 0)  return SaleItemErrors.InvalidItem;
        if (quantity <= 0)          return SaleItemErrors.InvalidQuantity;
        if (price < 0) return SaleItemErrors.InvalidPrice;

        SaleId = saleId;
        StoreItemUnitId = storeItemUnitId;
        Quantity = quantity;
        Price = price;

        return Result.Updated;
    }

    internal void IncreaseQuantity(decimal by) => Quantity += by; // caller validates

    public Result<Updated> AssignSale(Sale sale)
    {
        if (sale is null)
        {
            return SaleItemErrors.SaleIsRequired;
        }
        Sale = sale;
        SaleId = sale.Id;
        
        return Result.Updated;
    }
}
