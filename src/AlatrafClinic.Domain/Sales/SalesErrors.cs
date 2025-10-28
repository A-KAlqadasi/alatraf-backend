using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Sales;

public static class SaleErrors
{
    public static readonly Error DiagnosisRequired =
        Error.Validation("Sale.DiagnosisRequired", "A valid diagnosis must be specified for the sale.");

    public static readonly Error NoItemsProvided =
        Error.Validation("Sale.NoItemsProvided", "A sale must contain at least one sales item.");

    public static readonly Error InvalidSalesItem =
        Error.Validation("Sale.InvalidSalesItem", "Sales item cannot be null or invalid.");

    public static readonly Error SalesItemNotFound =
        Error.NotFound("Sale.SalesItemNotFound", "Sales item was not found in this sale.");

    public static readonly Error InvalidPayment =
        Error.Validation("Sale.InvalidPayment", "Invalid payment reference provided.");

    public static readonly Error InvalidExchangeOrder =
        Error.Validation("Sale.InvalidExchangeOrder", "Invalid exchange order reference provided.");

    public static readonly Error InvalidExitCard =
        Error.Validation("Sale.InvalidExitCard", "Invalid exit card reference provided.");

    public static readonly Error NotEditable =
        Error.Validation("Sale.NotEditable", "This sale cannot be modified because it is inactive or finalized.");

    public static readonly Error AlreadyInactive =
        Error.Validation("Sale.AlreadyInactive", "The sale is already inactive.");

    public static readonly Error ExitCardConflictWithExchangeOrder =
        Error.Validation("Sale.ExitCardConflict", "A sale cannot be linked to both an ExitCard and an ExchangeOrder.");

    public static readonly Error ExchangeOrderAlreadyAssigned =
        Error.Validation("Sale.ExchangeOrderAlreadyAssigned", "An exchange order is already assigned to this sale.");

    public static readonly Error ExitCardAlreadyAssigned =
        Error.Validation("Sale.ExitCardAlreadyAssigned", "An exit card is already assigned to this sale.");

    public static readonly Error LockedBySourceDocument =
        Error.Validation("Sale.LockedBySourceDocument", "This sale's items are locked because it is linked to an ExitCard or ExchangeOrder.");
}
