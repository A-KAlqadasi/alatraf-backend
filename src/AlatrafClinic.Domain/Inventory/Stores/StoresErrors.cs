using AlatrafClinic.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlatrafClinic.Domain.Inventory.Stores
{
    public static class StoresErrors
    {
        public static readonly Error NameRequired =
          Error.Validation("Inventory.Store.NameRequired", "Store name is required.");

        public static readonly Error StoreNotActive =
            Error.Validation("Inventory.Store.NotActive", "Store is not active.");

        public static readonly Error StoreAlreadyActive =
            Error.Validation("Inventory.Store.AlreadyActive", "Store is already active.");

        public static readonly Error DuplicateName =
            Error.Validation("Inventory.Store.DuplicateName", "Another store with the same name already exists.");

        public static readonly Error ItemAlreadyExists =
            Error.Validation("Inventory.Store.ItemAlreadyExists", "This item already exists in the store.");

        public static readonly Error CannotDeleteStore =
            Error.Conflict("Inventory.Store.CannotDelete", "Store cannot be deleted because it contains items or linked records.");

        public static readonly Error StoreNotFound =
            Error.NotFound("Inventory.Store.NotFound", "The specified store could not be found.");

    }
}
