using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards.Enums;

namespace AlatrafClinic.Domain.RepairCards.Orders;

public static class OrderErrors
{
    public static readonly Error InvalidSection     = Error.Validation("Order.InvalidSection", "Section is required.");
    public static readonly Error InvalidStore       = Error.Validation("Order.InvalidStore", "Store is required.");
    public static readonly Error InvalidRepairCard  = Error.Validation("Order.InvalidRepairCard", "Repair card is required for this order type.");
    public static readonly Error NoItems            = Error.Validation("Order.NoItems", "At least one order item is required.");
    public static readonly Error ReadOnly = Error.Validation("Order.ReadOnly", "Order is not editable in the current state.");
    public static readonly Error ExchangeOrderNumberRequired = Error.Validation("Order.ExchangeOrderNumberRequired", "Exchange order number is required.");
    public static readonly Error ItemsConflictInOrderAndExchangeOrder = Error.Conflict("Order.ItemsConflictInOrderAndExchangeOrder", "The items in the order do not match the items in the exchange order.");
    public static readonly Error QuantityExceedsAvailable = Error.Conflict("Order.QuantityExceedsAvailable", "Requested quantity exceeds available stock.");
}