namespace AlatrafClinic.Domain.Payments;

public enum PaymentReference : byte
{
    TherapyCardNew,
    TherapyCardRenew,
    TherapyCardDamagedReplacement,
    Repair,
    Sales
}
