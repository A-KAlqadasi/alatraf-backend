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
        Error.Validation("Payment.OverPayment", "Paid ammount and discount is over required total ammount");
    public static readonly Error InvalidDiagnosisId =
        Error.Validation("Payment.InvalidDiagnosisId", "Diagnosis Id is invalid.");
    public static readonly Error InvalidPaymentType = Error.Validation("Payment.InvalidPaymentType", "Payement Type is invalid");
    public static readonly Error PaidAmountLessThanTotal = Error.Conflict("Payment.PaidAmountLessThanTotal", "The paid ammount and discount less than total ammount");
    public static readonly Error InvalidAccountId = Error.Validation("Payment.InvalidAccountId", "Account Id is invalid");

}
