using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Inventory.Items;

public static class ItemUnitErrors
{
    public static readonly Error InvalidQuantity = Error.Validation("ItemUnit.InvalidQuantity", "quantity must be positive.");

    public static readonly Error NotEnoughQuantity = Error.Validation("ItemUnit.NotEnoughQuantity", "Not enough quantity in item unit.");
    public static readonly Error UnitRequired = Error.Validation("ItemUnit.UnitRequired", "Unit is required.");
    public static readonly Error InvalidPrice = Error.Validation("ItemUnit.InvalidPrice", "Price must be non-negative.");
    public static readonly Error InvalidConversionFactor = Error.Validation("ItemUnit.InvalidConversionFactor", "Conversion factor must be positive.");
    public static readonly Error ItemUnitNotFound = Error.NotFound("ItemUnit.NotFound", "Item unit not found");
}