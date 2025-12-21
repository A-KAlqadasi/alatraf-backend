using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.Payments.WoundedPayments;

namespace AlatrafClinic.Domain.WoundedCards;

public class WoundedCard : AuditableEntity<int>
{
    public string CardNumber { get; private set; } = default!;
    public DateOnly IssueDate { get; private set; }
    public DateOnly ExpirationDate { get; private set; }
    public string? CardImagePath { get; private set; }
    public int PatientId { get; private set; }
    public Patient Patient { get; set; } = default!;
    public bool IsExpired => ExpirationDate < AlatrafClinicConstants.TodayDate;

    private WoundedCard() { }
    private WoundedCard(string cardNumber, DateOnly expiration, int patientId, string? cardImagePath)
    {
        CardNumber = cardNumber;
        ExpirationDate = expiration;
        PatientId = patientId;
        CardImagePath = cardImagePath;
    }

    public static Result<WoundedCard> Create(string cardNumber, DateOnly issueDate, DateOnly expiration,  int patientId,string? cardImagePath)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return WoundedCardErrors.CardNumberIsRequired;
        }
        
        if (issueDate > AlatrafClinicConstants.TodayDate)
        {
            return WoundedCardErrors.IssueDateInvalid;
        }

        if (expiration <= AlatrafClinicConstants.TodayDate)
        {
            return WoundedCardErrors.CardIsExpired;
        }

        if (issueDate >= expiration)
        {
            return WoundedCardErrors.IssueAfterExpiration;
        }
        if (patientId <= 0)
        {
            return WoundedCardErrors.PatientIdInvalid;
        }

        return new WoundedCard(cardNumber, expiration, patientId, cardImagePath);
    }

    public Result<Updated> Update(string cardNumber, DateOnly issueDate, DateOnly expiration, int patientId, string? cardImagePath)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return WoundedCardErrors.CardNumberIsRequired;
        }
        
        if (issueDate > AlatrafClinicConstants.TodayDate)
        {
            return WoundedCardErrors.IssueDateInvalid;
        }

        if (expiration <= AlatrafClinicConstants.TodayDate)
        {
            return WoundedCardErrors.CardIsExpired;
        }

        if (issueDate >= expiration)
        {
            return WoundedCardErrors.IssueAfterExpiration;
        }

        if (patientId <= 0)
        {
            return WoundedCardErrors.PatientIdInvalid;
        }
        
        CardNumber = cardNumber;
        ExpirationDate = expiration;
        IssueDate = issueDate;
        CardImagePath = cardImagePath;
        PatientId = patientId;

        return Result.Updated;
    }
}