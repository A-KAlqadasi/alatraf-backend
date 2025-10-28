using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlatrafClinic.Domain.Inventory.Purchases
{
    public class PurchaseItem:AuditableEntity
    {
        public int PurchaseItemId { get; private set; }
        public int ItemId { get; private set; }
        public int InvoiceId { get; private set; }
        public decimal Quantity { get; private set; }
        public Unit Unit { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal Total { get; private set; }
        public string? Notes { get; private set; }
        private PurchaseItem() { }

        private PurchaseItem(int itemId, int invoiceId, decimal quantity, Unit unit,
                             decimal unitPrice, string? notes)
        {
            ItemId = itemId;
            InvoiceId = invoiceId;
            Quantity = quantity;
            Unit = unit;
            UnitPrice = unitPrice;
            Total = quantity * unitPrice;
            Notes = notes;
        }
        public static Result<PurchaseItem> Create(int itemId, int invoiceId, decimal quantity, Unit unit,
                                               decimal unitPrice, string? notes = null)
        {
            if (itemId <= 0)
                return PurchaseItemErrors.InvalidItem;

            if (invoiceId <= 0)
                return PurchaseItemErrors.InvalidInvoice;

            if (quantity <= 0)
                return PurchaseItemErrors.InvalidQuantity;

            if (unit == null)
                return PurchaseItemErrors.UnitRequired;

            if (unitPrice <= 0)
                return PurchaseItemErrors.InvalidUnitPrice;

            return new PurchaseItem(itemId, invoiceId, quantity, unit, unitPrice, notes);
           
        }
        public Result Update(decimal quantity, decimal unitPrice, DateTime? expiryDate = null, string? notes = null)
        {
            if (quantity <= 0)
                return PurchaseItemErrors.InvalidQuantity;

            if (unitPrice <= 0)
                return PurchaseItemErrors.InvalidUnitPrice;

            Quantity = quantity;
            UnitPrice = unitPrice;
            Total = quantity * unitPrice;
            ExpiryDate = expiryDate;
            Notes = notes;

            return Result.Success();
        }
    }
}
    