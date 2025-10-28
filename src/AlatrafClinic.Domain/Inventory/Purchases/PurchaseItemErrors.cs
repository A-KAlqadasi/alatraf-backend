using AlatrafClinic.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlatrafClinic.Domain.Inventory.Purchases
{
    public static class PurchaseItemErrors
    {
        public static readonly Error InvalidItem =
            Error.Validation("Inventory.PurchaseItem.InvalidItem", "Item ID must be greater than zero.");

        public static readonly Error InvalidInvoice =
            Error.Validation("Inventory.PurchaseItem.InvalidInvoice", "Invoice ID must be greater than zero.");

        public static readonly Error InvalidQuantity =
            Error.Validation("Inventory.PurchaseItem.InvalidQuantity", "Quantity must be greater than zero.");

        public static readonly Error InvalidUnitPrice =
            Error.Validation("Inventory.PurchaseItem.InvalidUnitPrice", "Unit price must be greater than zero.");

        public static readonly Error UnitRequired =
            Error.Validation("Inventory.PurchaseItem.UnitRequired", "Unit is required.");

        public static readonly Error InvalidTotal =
            Error.Validation("Inventory.PurchaseItem.InvalidTotal", "Total amount must be greater than zero.");

        public static readonly Error NotFound =
            Error.NotFound("Inventory.PurchaseItem.NotFound", "Purchase item not found.");
        public static readonly Error CannotUpdate =
    Error.Conflict("Inventory.PurchaseItem.CannotUpdate", "This purchase item cannot be updated.");


    }
}
