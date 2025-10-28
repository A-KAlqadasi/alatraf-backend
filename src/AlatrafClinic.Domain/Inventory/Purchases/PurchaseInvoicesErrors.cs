using AlatrafClinic.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlatrafClinic.Domain.Inventory.Purchases
{
    public static class PurchaseInvoicesErrors
    {

        public static readonly Error NumberRequired =
            Error.Validation("Inventory.PurchaseInvoice.NumberRequired", "Invoice number is required.");

        public static readonly Error InvalidSupplier =
            Error.Validation("Inventory.PurchaseInvoice.InvalidSupplier", "Supplier ID must be greater than zero.");

        public static readonly Error InvalidStore =
            Error.Validation("Inventory.PurchaseInvoice.InvalidStore", "Store ID must be greater than zero.");

        public static readonly Error InvalidTotal =
            Error.Validation("Inventory.PurchaseInvoice.InvalidTotal", "Invoice total must be greater than zero.");
       
        public static readonly Error NotFound =
         Error.NotFound("Inventory.PurchaseInvoice.NotFound", "Purchase invoice not found.");
     
        public static readonly Error CannotUpdateApproved =
             Error.Conflict("Inventory.PurchaseInvoice.CannotUpdateApproved",
             "Approved invoices cannot be modified.");


    }
}
