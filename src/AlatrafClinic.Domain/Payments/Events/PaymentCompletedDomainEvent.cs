using AlatrafClinic.Domain.Common;

namespace AlatrafClinic.Domain.Payments.Events;

public sealed record PaymentCompletedDomainEvent : DomainEvent
{
    public int PaymentId { get; }
    public int DiagnosisId { get; }

    public PaymentCompletedDomainEvent(int paymentId, int diagnosisId)
    {
        PaymentId = paymentId;
        DiagnosisId = diagnosisId;
    }
}
