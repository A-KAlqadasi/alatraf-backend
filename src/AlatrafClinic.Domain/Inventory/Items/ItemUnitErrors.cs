using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Inventory.Items;

public static class ItemUnitErrors
{
    public static readonly Error InvalidQuantity = Error.Validation("ItemUnit.InvalidQuantity", "quantity must be positive.");
    
    public static readonly Error NotEnoughQuantity = Error.Validation("ItemUnit.NotEnoughQuantity", "Not enough quantity in item unit.");
}