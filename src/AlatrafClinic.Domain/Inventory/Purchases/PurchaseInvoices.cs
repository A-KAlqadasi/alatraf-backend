using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlatrafClinic.Domain.Inventory.Purchases
{
    public class PurchaseInvoices:AuditableEntity
    {
        public int InvoiceId { get; private set; }
        public string Number { get; private set; } 
        public DateTime Date { get; private set; }
        public int SupplierId { get; private set; }
        public int StoreId { get; private set; }
        public decimal Total { get; private set; }
        private PurchaseInvoice() { }

        private PurchaseInvoice(string number, DateTime date, int supplierId, int storeId,
                                decimal total)
        {
            Number = number;
            Date = date;
            SupplierId = supplierId;
            StoreId = storeId;
            Total = total;
         
        }
        public static Result<PurchaseInvoices> Create(string number, DateTime date, int supplierId, int storeId,
                                                  decimal total)
        {
            if (string.IsNullOrWhiteSpace(number))
                return PurchaseInvoicesErrors.NumberRequired;

            if (supplierId <= 0)
                return PurchaseInvoicesErrors.InvalidSupplier;

            if (storeId <= 0)
                return PurchaseInvoicesErrors.InvalidStore;

            if (total <= 0)
                return PurchaseInvoicesErrors.InvalidTotal;
            return new PurchaseInvoice(number, date, supplierId, storeId, total);
          
        }
        public Result Update(string number, DateTime date, int supplierId, int storeId, decimal total)
        {
            if (string.IsNullOrWhiteSpace(number))
                return PurchaseInvoicesErrors.NumberRequired;

            if (supplierId <= 0)
                return PurchaseInvoicesErrors.InvalidSupplier;

            if (storeId <= 0)
                return PurchaseInvoicesErrors.InvalidStore;

            if (total <= 0)
                return PurchaseInvoicesErrors.InvalidTotal;


            Date = date;
            SupplierId = supplierId;
            StoreId = storeId;
            Total = total;

            return Result.Success();
        }

    }
    }
