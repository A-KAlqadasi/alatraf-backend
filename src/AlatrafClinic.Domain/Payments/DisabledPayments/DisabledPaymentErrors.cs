using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Payments.DisabledPayments;

public static class DisabledPaymentsErrors
{
    public static readonly Error DisabledCardIdIsRequired =
        Error.Validation(
            "DisabledPayment.DisabledCardIdIsRequired",
            "Disabled Card Id is required."
        );
    public static readonly Error PaymentIdIsRequired =
        Error.Validation(
            "DisabledPayment.PaymentIdIsRequired",
            "Payment Id is required."
        );
}