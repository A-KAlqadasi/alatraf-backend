using AlatrafClinic.Domain.Common;

namespace AlatrafClinic.Domain.Payments.Events;

public sealed record PaymentFailedDomainEvent : DomainEvent
{
    public int PaymentId { get; }
    public int SaleId { get; }
    public string Reason { get; }

    public PaymentFailedDomainEvent(int paymentId, int saleId, string reason)
    {
        PaymentId = paymentId;
        SaleId = saleId;
        Reason = reason;
    }
}
