using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Sales;

public static class SaleErrors
{
    public static readonly Error InvalidDiagnosisId = Error.Validation("Sale.InvalidDiagnosisId", "Invalid diagnosis id.");
    public static readonly Error StoreRequired = Error.Validation("Sale.StoreRequired", "Store is required.");
    public static readonly Error NoItems = Error.Validation("Sale.NoItems", "At least one sale item is required.");
    public static readonly Error NotDraft = Error.Validation("Sale.NotDraft", "Operation allowed only in Draft status.");
    public static readonly Error AlreadyPosted = Error.Validation("Sale.AlreadyPosted", "Sale is already posted.");
    public static readonly Error AlreadyCancelled = Error.Validation("Sale.AlreadyCancelled", "Sale is already cancelled.");
    public static readonly Error WrongStore = Error.Validation("Sale.WrongStore", "All items must belong to the sale's store.");
    public static readonly Error PaymentRequired = Error.Validation("Sale.PaymentRequired", "Payment must be assigned before posting.");
    public static readonly Error ExchangeOrderRequired = Error.Validation("Sale.ExchangeOrderRequired", "Exchange order id is required to post.");
    public static readonly Error InvalidPayment = Error.Validation("Sale.InvalidPayment", "Invalid payment id.");
    public static readonly Error InvalidExchangeOrder = Error.Validation("Sale.InvalidExchangeOrder", "Invalid exchange order id.");
    public static readonly Error InvalidSaleItem = Error.Validation("Sale.InvalidSaleItem", "Sale item is required.");
    public static readonly Error SaleItemNotFound = Error.Validation("Sale.SaleItemNotFound", "Sale item not found.");
    public static readonly Error InvalidExitCardId = Error.Validation("Sale.InvalidExitCardId", "Invalid exit card id.");
    public static readonly Error ItemsConflictInOrderAndExchangeOrder = Error.Validation("Sale.ItemsConflictInOrderAndExchangeOrder", "Items in sale and exchange order do not match.");
    public static readonly Error QuantityExceedsAvailable = Error.Validation("Sale.QuantityExceedsAvailable", "One or more items exceed available quantity in store.");
    public static readonly Error ExitCardAlreadyAssigned = Error.Validation("Sale.ExitCardAlreadyAssigned", "Exit card is already assigned to this sale.");
    public static readonly Error SaleItemsAreRequired = Error.Validation("Sale.SaleItemsAreRequired", "At least one sale item is required.");
}
