using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Inventory.ExchangeOrders;

public static class ExchangeOrderErrors
{
    public static readonly Error StoreRequired = Error.Validation("ExchangeOrder.StoreRequired", "Store is required to create an exchange order.");
    public static readonly Error AlreadyApproved = Error.Validation("ExchangeOrder.AlreadyApproved", "Exchange order is already approved.");
    public static readonly Error InvalidItem = Error.Validation("ExchangeOrder.InvalidItem", "Invalid item in exchange order.");
    public static readonly Error InvalidQuantity = Error.Validation("ExchangeOrder.InvalidQuantity", "Quantity must be greater than zero.");
    public static readonly Error OrderIsRequired = Error.Validation("ExchangeOrder.OrderIsRequired", "Order is required to link with exchange order.");
    public static readonly Error SaleIsRequired = Error.Validation("ExchangeOrder.SaleIsRequired", "Sale is required to link with exchange order.");
    public static readonly Error ExchangeOrderRequired = Error.Validation("ExchangeOrder.ExchangeOrderRequired", "Exchange order is required.");
    public static readonly Error ExchangeOrderNumberRequired = Error.Validation("ExchangeOrder.ExchangeOrderNumberRequired", "Exchange order number is required.");
    public static readonly Error ExchangeOrderAlreadyAssignedToSales = Error.Conflict("ExchangeOrder.ExchangeOrderAlreadyAssignedToSales", "This exchange order is already assigned to sales and cannot be linked to an order.");

    public static readonly Error ExchangeOrderAlreadyAssignedToOrder = Error.Conflict("ExchangeOrder.ExchangeOrderAlreadyAssignedToOrder", "This exchange order is already assigned to an order and cannot be linked to a sale.");

}