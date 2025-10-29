using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Inventory.ExchangeOrders;

public static class ExchangeOrderErrors
{
    public static readonly Error StoreRequired = Error.Validation("ExchangeOrder.StoreRequired", "Store is required to create an exchange order.");
    public static readonly Error NoItems = Error.Validation("ExchangeOrder.NoItems", "At least one item is required to create an exchange order.");
    public static readonly Error AlreadyApproved = Error.Validation("ExchangeOrder.AlreadyApproved", "Exchange order is already approved.");
    public static readonly Error InvalidItem = Error.Validation("ExchangeOrder.InvalidItem", "Invalid item in exchange order.");
    public static readonly Error InvalidQuantity = Error.Validation("ExchangeOrder.InvalidQuantity", "Quantity must be greater than zero.");
}