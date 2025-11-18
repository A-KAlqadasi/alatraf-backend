using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Cards.DisabledCards;

namespace AlatrafClinic.Domain.Payments.DisabledPayments;

public class DisabledPayment : AuditableEntity<int>
{
    public int DisabledCardId { get; private set; }
    public DisabledCard? DisabledCard { get; set; }
    public int PaymentId { get; private set; }
    public Payment? Payment { get; set; }
    public string? Notes { get; private set; }
    
    
    private DisabledPayment()
    {
    }

    private DisabledPayment(int disabledCardId, int paymentId, string? notes):base(paymentId)
    {
        DisabledCardId = disabledCardId;
        PaymentId = paymentId;
        Notes = notes;
    }

    public static Result<DisabledPayment> Create(
        int disabledCardId,
        int paymentId,
        string? notes = null)
    {
        if (disabledCardId <= 0)
        {
            return DisabledPaymentsErrors.DisabledCardIdIsRequired;
        }
        if (paymentId <= 0)
        {
            return DisabledPaymentsErrors.PaymentIdIsRequired;
        }

        return new DisabledPayment(
            disabledCardId,
            paymentId, notes);
    }
    
    public Result<Updated> Updated(
        int disabledCardId,
        int paymentId,
        string? notes = null)
    {
        if (disabledCardId <= 0)
        {
            return DisabledPaymentsErrors.DisabledCardIdIsRequired;
        }

        if (paymentId <= 0)
        {
            return DisabledPaymentsErrors.PaymentIdIsRequired;
        }

        DisabledCardId = disabledCardId;
        PaymentId = paymentId;
        Notes = notes;

        return Result.Updated;
    }
}