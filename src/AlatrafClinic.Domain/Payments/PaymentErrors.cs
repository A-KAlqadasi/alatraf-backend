using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Payments;
public static class PaymentErrors
{
    public static readonly Error InvalidTotal =
        Error.Validation("Payment.InvalidTotal", "Total amount is invalid");

    public static readonly Error InvalidPaid =
        Error.Validation("Payment.InvalidPaid", "Paid amount must be positive.");

    public static readonly Error InvalidDiscount =
        Error.Validation("Payment.InvalidDiscount", "Discount cannot be negative.");

    public static readonly Error OverPayment =
        Error.Validation("Payment.OverPayment", "Paid ammount and discount is over required total ammount");
    public static readonly Error InvalidDiagnosisId =
        Error.Validation("Payment.InvalidDiagnosisId", "Diagnosis Id is invalid.");
    public static readonly Error InvalidPaymentReference = Error.Validation("Payment.InvalidPaymentReference", "Payement Reference is invalid");
    public static readonly Error PaidAmountLessThanTotal = Error.Conflict("Payment.PaidAmountLessThanTotal", "The paid ammount and discount less than total ammount");
    public static readonly Error InvalidAccountId = Error.Validation("Payment.InvalidAccountId", "Account Id is invalid");
    public static readonly Error PaymentAlreadyCompleted = Error.Conflict("Payment.PaymentAlreadyCompleted", "The payment is already completed");
    public static readonly Error InvalidPatientPayment = Error.Validation("Payment.InvalidPatientPayment", "Patient payments must be of type Patient");
    public static readonly Error InvalidDisabledPayment = Error.Validation("Payment.InvalidDisabledPayment", "Disabled payments must be of type Disabled");
    public static readonly Error InvalidWoundedPayment = Error.Validation("Payment.InvalidWoundedPayment", "Wounded payments must be of type Wounded");
    public static readonly Error PaymentNotFound = Error.NotFound("Payment.PaymentNotFound", "Payment not found");
    public static readonly Error DiagnosisMissmatch = Error.Conflict("Payment.DiagnosisMismatch", "Payment diagnosis does not match");
    public static readonly Error TotalMissmatch = Error.Conflict("Payment.TotalMismatch", "Payment total does not match");
    public static readonly Error InvalidAccountKind = Error.Validation("Payment.InvalidAccountKind", "Invalid account kind");
    public static readonly Error InvalidTicketId = Error.Validation("Payment.InvalidTicketId", "Ticket Id is invalid");
}
