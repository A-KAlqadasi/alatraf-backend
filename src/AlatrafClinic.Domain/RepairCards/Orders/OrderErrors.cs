using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards.Enums;

namespace AlatrafClinic.Domain.RepairCards.Orders;

public static class OrderErrors
{
    public static readonly Error OrderItemsAreRequired = Error.Validation("Order.ItemsRequired", "Order must have at least one item.");

    public static readonly Error SectionIdInvalid = Error.Validation("Order.SectionIdInvalid", "Section ID is invalid.");
    public static readonly Error ReadOnly = Error.Conflict("Order.ReadOnly", "Order is read-only and cannot be modified.");
    public static readonly Error OrderCannotCompleteUntilHasExchangeOrder = Error.Validation("Order.OrderCannotCompleteUntilHasExchangeOrder", "Order cannot be completed until it has an Exchange Order.");
    public static readonly Error InvalidExchangeOrderId = Error.Validation("Order.InvalidExchangeOrderId", "Invalid Exchange Order ID.");
}