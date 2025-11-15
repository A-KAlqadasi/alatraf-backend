using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Payments;
public static class PaymentErrors
{
    public static readonly Error InvalidTotal =
        Error.Validation("Payment.InvalidTotal", "Total amount must be greater than zero.");

    public static readonly Error InvalidPaid =
        Error.Validation("Payment.InvalidPaid", "Paid amount must be positive.");

    public static readonly Error InvalidDiscount =
        Error.Validation("Payment.InvalidDiscount", "Discount cannot be negative.");

    public static readonly Error OverPayment =
        Error.Validation("Payment.OverPayment", "Total of paid amount and discount cannot exceed total amount.");

    public static readonly Error AlreadyLinked =
        Error.Conflict("Payment.AlreadyLinked", "This payment is already linked to another aggregate.");

    public static readonly Error InvalidTypeForSale =
        Error.Validation("Payment.InvalidTypeForSale", "Payment type must be 'Sales' to link with a Sale.");

    public static readonly Error InvalidTypeForTherapy =
        Error.Validation("Payment.InvalidTypeForTherapy", "Payment type must be 'Therapy' to link with a Therapy Card status.");

    public static readonly Error InvalidTypeForRepair =
        Error.Validation("Payment.InvalidTypeForRepair", "Payment type must be 'Repair' to link with a Repair Card.");
    public static readonly Error InvalidDiagnosisId =
        Error.Validation("Payment.InvalidDiagnosisId", "Diagnosis Id is invalid.");
}
