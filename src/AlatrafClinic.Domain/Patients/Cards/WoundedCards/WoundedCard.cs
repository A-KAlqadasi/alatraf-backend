using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Payments.WoundedPayments;

namespace AlatrafClinic.Domain.Patients.Cards.WoundedCards;

public class WoundedCard : AuditableEntity<int>
{
    public string? CardNumber { get; set; }
    public DateTime? Expiration { get; set; }
    public string? CardImagePath { get; set; }
    public int? PatientId { get; set; }
    public Patient? Patient { get; set; }
    public ICollection<WoundedPayment> DisabledPayments { get; set; } = new List<WoundedPayment>();

    private WoundedCard() { }
    private WoundedCard(string? cardNumber, DateTime? expiration, string? cardImagePath, int? patientId)
    {
        CardNumber = cardNumber;
        Expiration = expiration;
        CardImagePath = cardImagePath;
        PatientId = patientId;
    }

    public static Result<WoundedCard> Create(string? cardNumber, DateTime? expiration, string? cardImagePath, int? patientId)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return WoundedCardErrors.CardNumberIsRequired;
        }
        if (!expiration.HasValue)
        {
            return WoundedCardErrors.ExpirationIsRequired;
        }
        if (expiration.Value <= DateTime.UtcNow)
        {
            return WoundedCardErrors.ExpirationMustBeInTheFuture;
        }
        if (!patientId.HasValue)
        {
            return WoundedCardErrors.PatientIdIsRequired;
        }

        return new WoundedCard(cardNumber, expiration, cardImagePath, patientId);
    }

    public Result<Updated> Update(string? cardNumber, DateTime? expiration, string? cardImagePath, int? patientId)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return WoundedCardErrors.CardNumberIsRequired;
        }
        if (!expiration.HasValue)
        {
            return WoundedCardErrors.ExpirationIsRequired;
        }
        if (expiration.Value <= DateTime.UtcNow)
        {
            return WoundedCardErrors.ExpirationMustBeInTheFuture;
        }
        if (!patientId.HasValue)
        {
            return WoundedCardErrors.PatientIdIsRequired;
        }
        
        CardNumber = cardNumber;
        Expiration = expiration;
        CardImagePath = cardImagePath;
        PatientId = patientId;

        return Result.Updated;
    }
}