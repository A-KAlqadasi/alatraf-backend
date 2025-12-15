using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Inventory.Stores;

public static class StoreItemUnitErrors
{
    public static readonly Error StoreIsRequired =
        Error.Validation("StoreItem.StoreIsRequired", "Store reference is required.");

    public static readonly Error ItemIsRequired =
        Error.Validation("StoreItem.ItemIsRequired", "Item reference is required.");

    public static readonly Error InvalidQuantity =
        Error.Validation("StoreItem.InvalidQuantity", "Quantity must be zero or greater.");

    public static readonly Error InvalidPrice =
        Error.Validation("StoreItem.InvalidPrice", "Local price must be greater than or equal to zero.");

    public static readonly Error QuantityNotEnough =
        Error.Validation("StoreItem.QuantityNotEnough", "Not enough quantity in stock.");

    public static readonly Error AlreadyExists =
        Error.Conflict("StoreItem.AlreadyExists", "This item already exists in the store inventory.");

    public static readonly Error NotFound =
        Error.NotFound("StoreItem.NotFound", "The store item could not be found.");

    public static readonly Error CannotDelete =
        Error.Conflict("StoreItem.CannotDelete", "Store item cannot be deleted due to active references.");

    public static readonly Error CannotAdjustStock =
    Error.Conflict("StoreItem.CannotAdjustStock", "Stock adjustment cannot be performed for this item.");
    public static readonly Error UnitIsRequired =
    Error.Validation("StoreItem.UnitIsRequired", "Unit reference is required.");
    public static readonly Error ItemUnitIsRequired =
Error.Validation("StoreItem.ItemUnitIsRequired", "ItemUnit reference is required.");
    public static readonly Error StoreItemUnitNotFound =
Error.NotFound("StoreItem.StoreItemUnitNotFound", "Store item unit not found.");
public static readonly Error ItemUnitNotFound =
Error.NotFound("StoreItem.ItemUnitNotFound", "Item unit not found.");
}
