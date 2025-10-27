using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.RepairCards.Orders;

public static class OrderItemErrors
{
    public static readonly Error QuantityInvalid = Error.Validation("OrderItem.QuantityInvalid", "Quantity must be greater than zero.");
    public static readonly Error PriceInvalid = Error.Validation("OrderItem.PriceInvalid", "Price must be greater than zero.");

    public static readonly Error ItemUnitIdInvalid = Error.Validation("OrderItem.ItemUnitIdInvalid", "Item Unit ID is invalid.");

}