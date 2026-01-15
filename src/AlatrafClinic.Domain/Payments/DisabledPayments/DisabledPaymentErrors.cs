using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Payments.DisabledPayments;

public static class DisabledPaymentsErrors
{
    public static readonly Error DisabledCardIdIsRequired =
        Error.Validation(
            "DisabledPayment.DisabledCardIdIsRequired",
            "معرف بطاقة المعاق مطلوب"
        );
    public static readonly Error PaymentIdIsRequired =
        Error.Validation(
            "DisabledPayment.PaymentIdIsRequired",
            "معرف الدفع مطلوب"
        );
}