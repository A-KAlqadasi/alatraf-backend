using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlatrafClinic.Domain.Inventory.Suppliers
{
    public class Suppliers : AuditableEntity
    {
        public int SuplierId { get; private set; }
        public string SuplierName { get; private set; }
        public string Phone { get; private set; }

        private Suppliers() { }
        private Suppliers(string suplierName, string phone)
        {
            SuplierName = SuplierName;
            Phone = phone;
        }
        public static Result<Suppliers> Create(string name, string phone)
        {
            if (string.IsNullOrWhiteSpace(name))
                return SupplierErrors.NameRequired;

            if (string.IsNullOrWhiteSpace(phone))
                return SupplierErrors.PhoneRequired;

            return new Supplier(name, phone);

        }
    }
}
