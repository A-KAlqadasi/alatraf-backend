namespace AlatrafClinic.Domain.Payments;

public enum PaymentType : byte
{
    Therapy = 0, // Payment for therapy cards or sessions
    Repair = 1,  // Payment for repair cards
    Sales = 2    // Payment for product sales
}
