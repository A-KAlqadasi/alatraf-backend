using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Payments.WoundedPayments;


public static class WoundedPaymentErrors
{
    public static readonly Error WoundedCardIdIsRequired =
        Error.Validation(
            "WoundedPayment.WoundedCardIdIsRequired",
            "Wounded Card Id is required."
        );

    public static readonly Error PaymentIdIsRequired =
        Error.Validation(
            "WoundedPayment.PaymentIdIsRequired",
            "Payment Id is required."
        );

    public static readonly Error TotalIsRequired =
        Error.Validation(
            "WoundedPayment.TotalIsRequired",
            "Total is required."
        );

    public static readonly Error ReportNumberIsRequired =
        Error.Validation(
            "WoundedPayment.ReportNumberIsRequired",
            "Report Number is required."
        );
    public static readonly Error MinimumPriceForReportNumberIsRequired =
        Error.Validation(
            "WoundedPayment.MinimumPriceForReportNumberIsRequired",
            "Minimum Price For Report Number is required."
        );
    public static readonly Error WoundedPaymentNotFound =
        Error.NotFound(
            "WoundedPayment.NotFound",
            "Wounded Payment not found."
        );
}