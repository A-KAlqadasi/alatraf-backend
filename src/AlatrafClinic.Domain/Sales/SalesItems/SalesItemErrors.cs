
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Sales.SalesItems;
public static class SalesItemErrors
{
    public static readonly Error ItemRequired =
        Error.Validation("SalesItem.ItemRequired", "Item is required for the sales item.");

    public static readonly Error UnitRequired =
        Error.Validation("SalesItem.UnitRequired", "Unit is required for the sales item.");

    public static readonly Error InvalidQuantity =
        Error.Validation("SalesItem.InvalidQuantity", "Quantity must be greater than zero.");

    public static readonly Error InvalidPrice =
        Error.Validation("SalesItem.InvalidPrice", "Price cannot be negative.");
}