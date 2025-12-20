using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.Payments.DisabledPayments;

namespace AlatrafClinic.Domain.DisabledCards;

public class DisabledCard : AuditableEntity<int>
{
    public string CardNumber { get; private set; } = default!;
    public DateOnly IssueDate { get; private set; }
    public DateOnly ExpirationDate { get; private set; }
    public string? CardImagePath { get; private set; }
    public int PatientId { get; private set; } 
    public Patient Patient { get; set; } = default!;
    public bool IsExpired => ExpirationDate < AlatrafClinicConstants.TodayDate;
    public ICollection<DisabledPayment> DisabledPayments { get; set; } = new List<DisabledPayment>();

    private DisabledCard() { }
    private DisabledCard(string cardNumber, DateOnly issueDate, DateOnly expiration, int patientId, string? cardImagePath = null)
    {
        CardNumber = cardNumber;
        IssueDate = issueDate;
        ExpirationDate = expiration;
        CardImagePath = cardImagePath;
        PatientId = patientId;
    }

    public static Result<DisabledCard> Create(string cardNumber, DateOnly issueDate, DateOnly expiration, int patientId, string? cardImagePath)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return DisabledCardErrors.CardNumberIsRequired;
        }

        if (issueDate > AlatrafClinicConstants.TodayDate)
        {
            return DisabledCardErrors.IssueDateInvalid;
        }

        if (expiration <= AlatrafClinicConstants.TodayDate)
        {
            return DisabledCardErrors.CardIsExpired;
        }

        if (issueDate >= expiration)
        {
            return DisabledCardErrors.IssueAfterExpiration;
        }

        if (patientId <= 0)
        {
            return DisabledCardErrors.PatientIdIsRequired;
        }

        return new DisabledCard(cardNumber, issueDate, expiration, patientId, cardImagePath);
    }

    public Result<Updated> Update(string cardNumber, DateOnly issueDate, DateOnly expiration, int patientId, string? cardImagePath)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return DisabledCardErrors.CardNumberIsRequired;
        }

        if (issueDate > AlatrafClinicConstants.TodayDate)
        {
            return DisabledCardErrors.IssueDateInvalid;
        }

        if (expiration <= AlatrafClinicConstants.TodayDate)
        {
            return DisabledCardErrors.CardIsExpired;
        }

        if (issueDate >= expiration)
        {
            return DisabledCardErrors.IssueAfterExpiration;
        }
        
        if (patientId <= 0)
        {
            return DisabledCardErrors.PatientIdIsRequired;
        }
        
        CardNumber = cardNumber;
        ExpirationDate = expiration;
        IssueDate = issueDate;
        CardImagePath = cardImagePath;
        PatientId = patientId;

        return Result.Updated;
    }
}