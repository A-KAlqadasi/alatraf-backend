using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.TherapyCards.Enums;

namespace AlatrafClinic.Domain.TherapyCards;

public class TherapyCardStatus : AuditableEntity<int>
{
    public int TherapyCardId { get; private set; }
    public TherapyCard? TherapyCard { get; private set; }

    public CardStatus Status { get; private set; }

    public int? PaymentId { get; private set; }
    public Payment? Payment { get; private set; }
    private TherapyCardStatus()
    {

    }
    private TherapyCardStatus(int therapyCardId, CardStatus status)
    {
        TherapyCardId = therapyCardId;
        Status = status;
    }

    public static Result<TherapyCardStatus> Create(int therapyCardId, CardStatus status)
    {
        if (!Enum.IsDefined(typeof(CardStatus), status))
        {
            return TherapyCardStatusErrors.InvalidStatus;
        }
        return new TherapyCardStatus(therapyCardId, status);
    }

    public Result<Updated> AssignPayment(Payment payment)
    {
        if (payment is null) return TherapyCardStatusErrors.InvalidPayment;

        if (payment.Type != PaymentType.Therapy) return TherapyCardStatusErrors.InvalidPaymentType;
        
        Payment = payment;
        PaymentId = payment.Id;
        
        return Result.Updated;
    }
}