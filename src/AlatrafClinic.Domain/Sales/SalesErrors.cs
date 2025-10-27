using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Sales;

public static class SalesErrors
{
    public static readonly Error DiagnosisRequired =
        Error.Validation("Sales.DiagnosisRequired", "Diagnosis is required for this sale.");

    public static readonly Error NoItems =
        Error.Validation("Sales.NoItems", "Sales must include at least one item.");

    public static readonly Error InvalidTotal =
        Error.Validation("Sales.InvalidTotal", "Total must be greater than zero.");

    public static readonly Error ItemAlreadyExists =
        Error.Conflict("Sales.ItemAlreadyExists", "The item already exists in the sale.");

    public static readonly Error InvalidPayment =
        Error.Validation("Sales.InvalidPayment", "Invalid payment identifier.");

    public static readonly Error PaymentAlreadyLinked =
        Error.Conflict("Sales.PaymentAlreadyLinked", "This sale is already linked to a payment.");

    public static readonly Error InvalidExitCard =
        Error.Validation("Sales.InvalidExitCard", "Invalid exit card identifier.");

    public static readonly Error ExitCardAlreadyLinked =
        Error.Conflict("Sales.ExitCardAlreadyLinked", "This sale is already linked to an exit card.");
}


