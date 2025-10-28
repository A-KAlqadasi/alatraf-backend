using AlatrafClinic.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlatrafClinic.Domain.Inventory.Stores
{
    public static class StoreItemsErrors
    {
        public static readonly Error StoreRequired =
            Error.Validation("Inventory.StoreItem.StoreRequired", "Store reference is required.");

        public static readonly Error ItemRequired =
            Error.Validation("Inventory.StoreItem.ItemRequired", "Item reference is required.");

        public static readonly Error InvalidQuantity =
            Error.Validation("Inventory.StoreItem.InvalidQuantity", "Quantity must be zero or greater.");

        public static readonly Error InvalidPrice =
            Error.Validation("Inventory.StoreItem.InvalidPrice", "Local price must be greater than or equal to zero.");

        public static readonly Error QuantityNotEnough =
            Error.Validation("Inventory.StoreItem.QuantityNotEnough", "Not enough quantity in stock.");

        public static readonly Error AlreadyExists =
            Error.Conflict("Inventory.StoreItem.AlreadyExists", "This item already exists in the store inventory.");

        public static readonly Error NotFound =
            Error.NotFound("Inventory.StoreItem.NotFound", "The store item could not be found.");

        public static readonly Error CannotDelete =
            Error.Conflict("Inventory.StoreItem.CannotDelete", "Store item cannot be deleted due to active references.");

    }
}
