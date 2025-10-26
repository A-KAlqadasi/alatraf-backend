using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Payments.DisabledPayments;

namespace AlatrafClinic.Domain.Patients.Cards.DisabledCards;

public class DisabledCard : AuditableEntity<int>
{
    public string? CardNumber { get; set; }
    public DateTime? Expiration { get; set; }
    public string? CardImagePath { get; set; }
    public int? PatientId { get; set; }
    public Patient? Patient { get; set; }
    public ICollection<DisabledPayment> DisabledPayments { get; set; } = new List<DisabledPayment>();

    private DisabledCard() { }
    private DisabledCard(string? cardNumber, DateTime? expiration, string? cardImagePath, int? patientId)
    {
        CardNumber = cardNumber;
        Expiration = expiration;
        CardImagePath = cardImagePath;
        PatientId = patientId;
    }

    public static Result<DisabledCard> Create(string? cardNumber, DateTime? expiration, string? cardImagePath, int? patientId)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return DisabledCardErrors.CardNumberIsRequired;
        }
        if (!expiration.HasValue)
        {
            return DisabledCardErrors.ExpirationIsRequired;
        }
        if (expiration.Value <= DateTime.UtcNow)
        {
            return DisabledCardErrors.ExpirationMustBeInTheFuture;
        }
        if (!patientId.HasValue)
        {
            return DisabledCardErrors.PatientIdIsRequired;
        }

        return new DisabledCard(cardNumber, expiration, cardImagePath, patientId);
    }

    public Result<Updated> Update(string? cardNumber, DateTime? expiration, string? cardImagePath, int? patientId)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return DisabledCardErrors.CardNumberIsRequired;
        }
        if (!expiration.HasValue)
        {
            return DisabledCardErrors.ExpirationIsRequired;
        }
        if (expiration.Value <= DateTime.UtcNow)
        {
            return DisabledCardErrors.ExpirationMustBeInTheFuture;
        }
        if (!patientId.HasValue)
        {
            return DisabledCardErrors.PatientIdIsRequired;
        }
        
        CardNumber = cardNumber;
        Expiration = expiration;
        CardImagePath = cardImagePath;
        PatientId = patientId;

        return Result.Updated;
    }
}