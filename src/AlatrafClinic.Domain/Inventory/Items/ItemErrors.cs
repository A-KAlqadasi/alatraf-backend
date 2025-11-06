using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Inventory.Items;

public static class ItemErrors
{
    public static readonly Error NameIsRequired = Error.Validation("Item.NameIsRequired", "Item name is required.");
    public static readonly Error ItemUnitsAreRequired = Error.Validation("Item.ItemUnitsAreRequired", "At least one item unit is required.");
    public static readonly Error BaseUnitIsRequired = Error.Validation("Item.BaseUnitIsRequired", "Base unit is required.");
    public static readonly Error AlreadyInactive = Error.Validation("Item.AlreadyInactive", "Item is already inactive.");
}