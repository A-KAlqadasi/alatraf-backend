using AlatrafClinic.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlatrafClinic.Domain.Inventory.Stores.Items
{
    public static class ItemsErrors
    {

        public static readonly Error NameRequired =
             Error.Validation("Inventory.Item.NameRequired", "Item name is required.");

        public static readonly Error InvalidPrice =
            Error.Validation("Inventory.Item.InvalidPrice", "Item price must be greater than zero.");

        public static readonly Error InvalidQuantity =
            Error.Validation("Inventory.Item.InvalidQuantity", "Item quantity must be zero or greater.");

        public static readonly Error InvalidMinQuantity =
            Error.Validation("Inventory.Item.InvalidMinQuantity", "Minimum quantity must be zero or greater.");

        public static readonly Error InvalidPriceRange =
            Error.Validation("Inventory.Item.InvalidPriceRange", "Minimum price to pay cannot be greater than maximum price to pay.");

        public static readonly Error UnitRequired =
            Error.Validation("Inventory.Item.UnitRequired", "Item unit is required.");

        public static readonly Error QuantityNotEnough =
            Error.Validation("Inventory.Item.QuantityNotEnough", "Item quantity is not enough to complete this operation.");

        public static readonly Error ItemNotActive =
            Error.Validation("Inventory.Item.NotActive", "Item is not active.");
        public static readonly Error ItemAlreadyActive =
            Error.Validation("Inventory.Item.AlreadyActive", "Item is Already Active .");

        public static readonly Error DuplicateName =
            Error.Validation("Inventory.Item.DuplicateName", "Another item with the same name already exists.");

        public static readonly Error CannotDeleteItem =
            Error.Conflict("Inventory.Item.CannotDelete", "Item cannot be deleted due to existing relations with other services.");

        public static readonly Error InvalidAmount = Error.Validation("Inventory.Item.InvalidAmount", "Amount must be greater than zero.");
    }
}
