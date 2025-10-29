using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Cards.DisabledCards;

namespace AlatrafClinic.Domain.Patients.Payments.DisabledPayments;

public class DisabledPayment : AuditableEntity<int>
{
    public int? DisabledCardId { get; set; }
    public DisabledCard? DisabledCard { get; set; }
    public int? PaymentId { get; set; }
    //public Payment? Payment { get; set; }
    private DisabledPayment()
    {
    }

    private DisabledPayment(int? disabledCardId, int? paymentId)
    {
        DisabledCardId = disabledCardId;
        PaymentId = paymentId;
    }

    public static Result<DisabledPayment> Create(
        int? disabledCardId,
        int? paymentId)
    {
        if (disabledCardId is null || disabledCardId <= 0)
        {
            return DisabledPaymentsErrors.DisabledCardIdIsRequired;
        }
        if (paymentId is null || paymentId <= 0)
        {
            return DisabledPaymentsErrors.PaymentIdIsRequired;
        }

        return new DisabledPayment(
            disabledCardId,
            paymentId);
    }
    
    public Result<Updated> Updated(
        int? disabledCardId,
        int? paymentId)
    {
        if (disabledCardId is null || disabledCardId <= 0)
        {
            return DisabledPaymentsErrors.DisabledCardIdIsRequired;
        }

        if (paymentId is null || paymentId <= 0)
        {
            return DisabledPaymentsErrors.PaymentIdIsRequired;
        }

        DisabledCardId = disabledCardId;
        PaymentId = paymentId;

        return Result.Updated;
    }
}