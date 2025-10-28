using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Inventory.Purchases;
public static class PurchaseItemErrors
{
     public static readonly Error InvalidItem = Error.Validation("PurchaseItem.InvalidItem", "Store item unit is required.");
    public static readonly Error InvalidQuantity = Error.Validation("PurchaseItem.InvalidQuantity", "Quantity must be greater than zero.");
    public static readonly Error InvalidUnitPrice = Error.Validation("PurchaseItem.InvalidUnitPrice", "Unit price must be greater than zero.");
    public static readonly Error WrongStore = Error.Validation("PurchaseItem.WrongStore", "Purchase item must belong to the same store as the invoice.");
}