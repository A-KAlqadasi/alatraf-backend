using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Inventory.Stores;

public static class StoreErrors
{
    public static readonly Error NameIsRequired =
        Error.Validation("Store.NameIsRequired", "Store name is required.");

    public static readonly Error StoreNotActive =
        Error.Validation("Store.NotActive", "Store is not active.");

    public static readonly Error StoreAlreadyActive =
        Error.Validation("Store.AlreadyActive", "Store is already active.");

    public static readonly Error DuplicateName =
        Error.Validation("Store.DuplicateName", "Another store with the same name already exists.");

    public static readonly Error ItemAlreadyExists =
        Error.Validation("Store.ItemAlreadyExists", "This item already exists in the store.");

    public static readonly Error CannotDeleteStore =
        Error.Conflict("Store.CannotDelete", "Store cannot be deleted because it contains items or linked records.");

    public static readonly Error StoreNotFound =
        Error.NotFound("Store.NotFound", "The specified store could not be found.");

    public static readonly Error ItemUnitIsRequired =
        Error.Validation("Store.ItemUnitIsRequired", "Item unit is required.");
    public static readonly Error CannotDelete =
        Error.Conflict("Store.CannotDelete", "Store cannot be deleted because it contains items or linked records.");
}