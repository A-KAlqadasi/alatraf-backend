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

    private PurchaseItem(int invoiceId, StoreItemUnit storeItemUnit, decimal quantity, decimal unitPrice, string? notes)
    {
        PurchaseInvoiceId = invoiceId;
        StoreItemUnit = storeItemUnit;
        StoreItemUnitId = storeItemUnit.Id;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Notes = notes;
    }

    public static Result<PurchaseItem> Create(int invoiceId, StoreItemUnit storeItemUnit, decimal quantity, decimal unitPrice, string? notes = null)
    {
        if (invoiceId <= 0)
        {
            return PurchaseItemErrors.PurchaseInvoiceIsRequired;
        }
        if (storeItemUnit is null) return PurchaseItemErrors.InvalidItem;
        if (quantity <= 0) return PurchaseItemErrors.InvalidQuantity;
        if (unitPrice <= 0) return PurchaseItemErrors.InvalidUnitPrice;

        return new PurchaseItem(invoiceId, storeItemUnit, quantity, unitPrice, notes);
    }

    internal Result<Updated> Update(int invoiceId, StoreItemUnit storeItemUnit, decimal quantity, decimal unitPrice, string? notes)
    {
        if (invoiceId <= 0)
        {
            return PurchaseItemErrors.PurchaseInvoiceIsRequired;
        }

        if (storeItemUnit is null) return PurchaseItemErrors.InvalidItem;
        if (quantity <= 0) return PurchaseItemErrors.InvalidQuantity;
        if (unitPrice <= 0) return PurchaseItemErrors.InvalidUnitPrice;

        PurchaseInvoiceId = invoiceId;
        StoreItemUnit = storeItemUnit;
        StoreItemUnitId = storeItemUnit.Id;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Notes = notes;

        return Result.Updated;
    }

    internal void IncreaseQuantity(decimal by) => Quantity += by; // validated by caller

    public Result<Updated> AssignPurchaseInvoice(PurchaseInvoice invoice)
    {
        if (invoice is null)
        {
            return PurchaseItemErrors.PurchaseInvoiceIsRequired;
        }
        PurchaseInvoice = invoice;
        PurchaseInvoiceId = invoice.Id;
        return Result.Updated;
    }
}