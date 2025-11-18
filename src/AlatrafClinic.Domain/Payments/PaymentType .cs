namespace AlatrafClinic.Domain.Payments;

public enum PaymentType : byte
{
    NewTherapyCard = 1,
    RenewTherapyCard,
    DamagedReplacementTherapyCard,
    Repair,
    Sales
}
