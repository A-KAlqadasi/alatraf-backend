using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Payments.PatientPayments;

public static class PatientPaymentErrors
{
    public static readonly Error VoucherNumberIsRequired = Error.Validation(
        "PatientPayment.VoucherNumberIsRequired",
        "رقم سند الدفع مطلوب"
    );

    public static readonly Error PaymentIdIsRequired = Error.Validation(
        "PatientPayment.PaymentIdIsRequired",
        "معرف الدفع مطلوب"
    );
    public static readonly Error VoucherNumberAlreadyExists = Error.Validation(
        "PatientPayment.VoucherNumberAlreadyExists",
        "رقم سند الدفع موجود بالفعل"
    );
}