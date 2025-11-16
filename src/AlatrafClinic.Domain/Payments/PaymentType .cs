namespace AlatrafClinic.Domain.Payments;

public enum PaymentType : byte
{
    TherapyCardNew = 1,
    TherapyCardRenew,
    TherapyCardDamagedReplacement,
    Repair,
    Sales
}
