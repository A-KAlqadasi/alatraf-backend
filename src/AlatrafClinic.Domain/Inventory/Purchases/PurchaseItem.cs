using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Stores;
using AlatrafClinic.Domain.Inventory.Units;

namespace AlatrafClinic.Domain.Inventory.Purchases;

public class PurchaseItem : AuditableEntity<int>
{
   public int PurchaseInvoiceId { get; private set; }
    public PurchaseInvoice PurchaseInvoice { get; private set; } = default!;
    public int StoreItemUnitId { get; private set; }
    public StoreItemUnit StoreItemUnit { get; private set; } = default!;
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Total => Quantity * UnitPrice;

    public string? Notes { get; private set; } = string.Empty;

    private PurchaseItem() { }

    private PurchaseItem(StoreItemUnit storeItemUnit, decimal quantity, decimal unitPrice, string? notes)
    {
        StoreItemUnit = storeItemUnit;
        StoreItemUnitId = storeItemUnit.Id;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Notes = notes;
    }

    public static Result<PurchaseItem> Create(StoreItemUnit storeItemUnit, decimal quantity, decimal unitPrice, string? notes = null)
    {
        if (storeItemUnit is null) return PurchaseItemErrors.InvalidItem;
        if (quantity <= 0) return PurchaseItemErrors.InvalidQuantity;
        if (unitPrice <= 0) return PurchaseItemErrors.InvalidUnitPrice;

        return new PurchaseItem(storeItemUnit, quantity, unitPrice, notes);
    }

    internal Result<Updated> Update(StoreItemUnit storeItemUnit, decimal quantity, decimal unitPrice, string? notes)
    {
        if (storeItemUnit is null) return PurchaseItemErrors.InvalidItem;
        if (quantity <= 0) return PurchaseItemErrors.InvalidQuantity;
        if (unitPrice <= 0) return PurchaseItemErrors.InvalidUnitPrice;

        StoreItemUnit = storeItemUnit;
        StoreItemUnitId = storeItemUnit.Id;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Notes = notes;

        return Result.Updated;
    }

    internal void IncreaseQuantity(decimal by) => Quantity += by; // validated by caller
}