using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.RepairCards.Orders;

public static class OrderItemErrors
{
   public static readonly Error StoreItemRequired = Error.Validation("OrderItem.StoreItemRequired", "Store item unit is required.");
    public static readonly Error InvalidQuantity   = Error.Validation("OrderItem.InvalidQuantity", "Quantity must be greater than zero.");
    public static readonly Error InvalidPrice      = Error.Validation("OrderItem.InvalidPrice", "Price must be zero or greater.");

}