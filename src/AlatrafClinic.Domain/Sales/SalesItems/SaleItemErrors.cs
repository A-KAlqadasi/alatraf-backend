
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Sales.SalesItems;
public static class SaleItemErrors
{
   public static readonly Error InvalidItem      = Error.Validation("SaleItem.InvalidItem", "Store item unit is required.");
    public static readonly Error InvalidQuantity  = Error.Validation("SaleItem.InvalidQuantity", "Quantity must be greater than zero.");
    public static readonly Error InvalidPrice     = Error.Validation("SaleItem.InvalidPrice", "Price must be zero or greater.");
}