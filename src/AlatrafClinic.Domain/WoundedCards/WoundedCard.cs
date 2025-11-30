using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.Payments.WoundedPayments;

namespace AlatrafClinic.Domain.WoundedCards;

public class WoundedCard : AuditableEntity<int>
{
    public string CardNumber { get; private set; } = default!;
    public DateTime Expiration { get; private set; }
    public string? CardImagePath { get; private set; }
    public int PatientId { get; private set; }
    public Patient Patient { get; set; } = default!;
    public bool IsExpired => Expiration < DateTime.Now.Date;
    public ICollection<WoundedPayment> WoundedPayments { get; set; } = new List<WoundedPayment>();

    private WoundedCard() { }
    private WoundedCard(string cardNumber, DateTime expiration, int patientId, string? cardImagePath)
    {
        CardNumber = cardNumber;
        Expiration = expiration;
        PatientId = patientId;
        CardImagePath = cardImagePath;
    }

    public static Result<WoundedCard> Create(string cardNumber, DateTime expiration,  int patientId,string? cardImagePath)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return WoundedCardErrors.CardNumberIsRequired;
        }
        
        if (expiration < DateTime.Now.Date)
        {
            return WoundedCardErrors.CardIsExpired;
        }
        if (patientId <= 0)
        {
            return WoundedCardErrors.PatientIdInvalid;
        }

        return new WoundedCard(cardNumber, expiration, patientId, cardImagePath);
    }

    public Result<Updated> Update(string cardNumber, DateTime expiration, int patientId, string? cardImagePath)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return WoundedCardErrors.CardNumberIsRequired;
        }
        
        if (expiration < DateTime.Now.Date)
        {
            return WoundedCardErrors.CardIsExpired;
        }
        if (patientId <= 0)
        {
            return WoundedCardErrors.PatientIdInvalid;
        }
        
        CardNumber = cardNumber;
        Expiration = expiration;
        CardImagePath = cardImagePath;
        PatientId = patientId;

        return Result.Updated;
    }
}