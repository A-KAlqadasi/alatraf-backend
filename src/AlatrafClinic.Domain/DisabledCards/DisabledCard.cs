using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.Payments.DisabledPayments;

namespace AlatrafClinic.Domain.DisabledCards;

public class DisabledCard : AuditableEntity<int>
{
    public string CardNumber { get; private set; } = default!;
    public DateTime ExpirationDate { get; private set; }
    public string? CardImagePath { get; private set; }
    public int PatientId { get; private set; } 
    public Patient Patient { get; set; } = default!;
    public bool IsExpired => ExpirationDate < DateTime.Now.Date;
    public ICollection<DisabledPayment> DisabledPayments { get; set; } = new List<DisabledPayment>();

    private DisabledCard() { }
    private DisabledCard(string cardNumber, DateTime expiration, int patientId, string? cardImagePath = null)
    {
        CardNumber = cardNumber;
        ExpirationDate = expiration;
        CardImagePath = cardImagePath;
        PatientId = patientId;
    }

    public static Result<DisabledCard> Create(string cardNumber, DateTime expiration, int patientId, string? cardImagePath)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return DisabledCardErrors.CardNumberIsRequired;
        }

        if (expiration < DateTime.Now.Date)
        {
            return DisabledCardErrors.CardIsExpired;
        }
        
        if (patientId <= 0)
        {
            return DisabledCardErrors.PatientIdIsRequired;
        }

        return new DisabledCard(cardNumber, expiration, patientId, cardImagePath);
    }

    public Result<Updated> Update(string cardNumber, DateTime expiration, int patientId, string? cardImagePath)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return DisabledCardErrors.CardNumberIsRequired;
        }

        if (expiration <= DateTime.Now.Date)
        {
            return DisabledCardErrors.CardIsExpired;
        }
        
        if (patientId <= 0)
        {
            return DisabledCardErrors.PatientIdIsRequired;
        }
        
        CardNumber = cardNumber;
        ExpirationDate = expiration;
        CardImagePath = cardImagePath;
        PatientId = patientId;

        return Result.Updated;
    }
}