using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Inventory.Purchases;

public static class PurchaseInvoiceErrors
{
   public static readonly Error NumberRequired = Error.Validation("PurchaseInvoice.NumberRequired", "Invoice number is required.");
    public static readonly Error InvalidSupplier = Error.Validation("PurchaseInvoice.InvalidSupplier", "Supplier is required.");
    public static readonly Error InvalidStore = Error.Validation("PurchaseInvoice.InvalidStore", "Store is required.");
    public static readonly Error ItemsRequired = Error.Validation("PurchaseInvoice.ItemsRequired", "At least one purchase item is required.");
    public static readonly Error MixedStores = Error.Validation("PurchaseInvoice.MixedStores", "All items must belong to the invoice store.");
    public static readonly Error NotDraft = Error.Validation("PurchaseInvoice.NotDraft", "Operation allowed only in Draft status.");
    public static readonly Error AlreadyPosted = Error.Validation("PurchaseInvoice.AlreadyPosted", "Invoice is already posted.");
    public static readonly Error NotPosted = Error.Validation("PurchaseInvoice.NotPosted", "Invoice is not posted.");
    public static readonly Error AlreadyCancelled = Error.Validation("PurchaseInvoice.AlreadyCancelled", "Invoice is already cancelled.");
}