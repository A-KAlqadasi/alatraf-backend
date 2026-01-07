using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;
using AlatrafClinic.Domain.Inventory.Stores;

namespace AlatrafClinic.Domain.Sales.SalesItems;

public class SaleItem : AuditableEntity<int>
{
    public int SaleId { get; private set; }
    public Sale Sale { get; set; } = default!;

    public int StoreItemUnitId { get; private set; }
    public StoreItemUnit StoreItemUnit { get; set; } = default!;
    public int ItemUnitId { get; private set; }
    public ItemUnit ItemUnit { get; set; } = default!;

    public decimal Quantity { get; private set; }
    public decimal Price { get; private set; }
    public decimal Total => Quantity * Price;

    private SaleItem() { }

    private SaleItem(int saleId, ItemUnit itemUnit, decimal quantity)
    {
        SaleId = saleId;
        ItemUnitId = itemUnit.Id;
        Quantity = quantity;
        Price = itemUnit.Price;
    }

    public static Result<SaleItem> Create(int saleId, ItemUnit itemUnit, decimal quantity)
    {
        if (itemUnit is null) return SaleItemErrors.InvalidItem;
        if (quantity <= 0) return SaleItemErrors.InvalidQuantity;

        return new SaleItem(saleId, itemUnit, quantity);
    }

    internal Result<Updated> Update(int saleId, ItemUnit itemUnit, decimal quantity)
    {
        if (saleId <= 0) return SaleItemErrors.InvalidSaleId;
        if (itemUnit is null) return SaleItemErrors.InvalidItem;
        if (quantity <= 0) return SaleItemErrors.InvalidQuantity;

        SaleId = saleId;
        ItemUnit = itemUnit;
        ItemUnitId = itemUnit.Id;
        Quantity = quantity;
        Price = itemUnit.Price;

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
