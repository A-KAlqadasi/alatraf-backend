using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Payments.WoundedPayments;


public static class WoundedPaymentErrors
{
    public static readonly Error PaymentIdIsRequired =
        Error.Validation(
            "WoundedPayment.PaymentIdIsRequired",
            "Payment Id is required."
        );
    public static readonly Error ReportNumberIsRequired =
        Error.Validation(
            "WoundedPayment.ReportNumberIsRequired",
            "Report Number is required."
        );
}