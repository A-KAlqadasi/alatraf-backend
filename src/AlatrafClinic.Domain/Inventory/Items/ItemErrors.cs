using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Inventory.Items;

public static class ItemErrors
{
    public static readonly Error NameIsRequired = Error.Validation("Item.NameIsRequired", "Item name is required.");
    public static readonly Error ItemUnitsAreRequired = Error.Validation("Item.ItemUnitsAreRequired", "At least one item unit is required.");
}