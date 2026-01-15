using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Payments.WoundedPayments;


public static class WoundedPaymentErrors
{
    public static readonly Error PaymentIdIsRequired =
        Error.Validation(
            "WoundedPayment.PaymentIdIsRequired",
            "معرف الدفع مطلوب."
        );
    public static readonly Error ReportNumberIsRequired =
        Error.Validation(
            "WoundedPayment.ReportNumberIsRequired",
            "رقم البلاغ مطلوب"
        );
}