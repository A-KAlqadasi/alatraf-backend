
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Sales.SalesItems;

public static class SalesItemErrors
{
    public static readonly Error QuantityInvalid =
        Error.Validation("SalesItem.QuantityInvalid", "Quantity must be greater than zero.");

    public static readonly Error PriceInvalid =
        Error.Validation("SalesItem.PriceInvalid", "Price must be greater than zero.");

    public static readonly Error UnitRequired =
        Error.Validation(".UnitRequired", "Unit must be provided.");
   
     public static Error SaleIdRequired => Error.Validation(
        code: "SalesItemErrors.SaleRequired",
        description: "SaleID is required.");

         public static readonly Error ItemIdRequired =
        Error.Validation("SalesItem.ItemRequired", "Item is required .");

}