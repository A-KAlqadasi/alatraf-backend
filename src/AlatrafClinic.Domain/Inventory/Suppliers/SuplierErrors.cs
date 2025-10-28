using AlatrafClinic.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlatrafClinic.Domain.Inventory.Suppliers
{
    public static class SuplierErrors
    {

        public static readonly Error NameRequired =
            Error.Validation("Inventory.Supplier.NameRequired", "Supplier name is required.");

        public static readonly Error PhoneRequired =
            Error.Validation("Inventory.Supplier.PhoneRequired", "Supplier phone number is required.");

        public static readonly Error NotFound =
            Error.NotFound("Inventory.Supplier.NotFound", "Supplier not found.");

    }
}
