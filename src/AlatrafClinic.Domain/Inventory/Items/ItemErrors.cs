using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Inventory.Items;

public static class ItemErrors
{
    public static readonly Error NameIsRequired = Error.Validation("Item.NameIsRequired", "Item name is required.");
    public static readonly Error ItemUnitsAreRequired = Error.Validation("Item.ItemUnitsAreRequired", "At least one item unit is required.");
    public static readonly Error BaseUnitIsRequired = Error.Validation("Item.BaseUnitIsRequired", "Base unit is required.");
    public static readonly Error AlreadyInactive = Error.Validation("Item.AlreadyInactive", "Item is already inactive.");
    public static readonly Error AlreadyActive = Error.Validation("Item.AlreadyActive", "Item is already active.");
    public static readonly Error CannotDeactivateWithExistingStock = Error.Validation("Item.CannotDeactivateWithExistingStock", "Cannot deactivate item with existing stock.");
    public static readonly Error CannotDeleteWithExistingStock = Error.Validation("Item.CannotDeleteWithExistingStock", "Cannot delete item with existing stock.");
    public static readonly Error ItemUnitNotFound = Error.NotFound("Item.ItemUnitNotFound", "The specified item unit was not found.");
    public static readonly Error NotFound = Error.NotFound("Item.NotFound", "The specified item was not found.");
}