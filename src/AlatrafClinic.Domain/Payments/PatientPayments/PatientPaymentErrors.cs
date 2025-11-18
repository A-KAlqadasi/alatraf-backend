using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Payments.PatientPayments;

public static class PatientPaymentErrors
{
    public static readonly Error VoucherNumberIsRequired = Error.Validation(
        "PatientPayment.VoucherNumberIsRequired",
        "Voucher number is required."
    );

    public static readonly Error PaymentIdIsRequired = Error.Validation(
        "PatientPayment.PaymentIdIsRequired",
        "Payment Id is required."
    );
}