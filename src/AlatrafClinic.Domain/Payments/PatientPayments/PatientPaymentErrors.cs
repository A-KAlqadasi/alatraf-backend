using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Payments.PatientPayments;

public static class PatientPaymentErrors
{
    public static readonly Error CouponNumberIsRequired = Error.Validation(
        "PatientPayment.CouponNumberIsRequired",
        "Coupon number is required."
    );

    public static readonly Error PaymentIdIsRequired = Error.Validation(
        "PatientPayment.PaymentIdIsRequired",
        "Payment Id is required."
    );
}