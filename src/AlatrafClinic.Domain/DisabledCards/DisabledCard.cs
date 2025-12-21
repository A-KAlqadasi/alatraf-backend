using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.Payments.DisabledPayments;

namespace AlatrafClinic.Domain.DisabledCards;

public class DisabledCard : AuditableEntity<int>
{
    public string CardNumber { get; private set; } = default!;
    public string DisabilityType { get; private set; } = default!;
    public DateOnly IssueDate { get; private set; }
    public string? CardImagePath { get; private set; }
    public int PatientId { get; private set; } 
    public Patient Patient { get; set; } = default!;
    public ICollection<DisabledPayment> DisabledPayments { get; set; } = new List<DisabledPayment>();

    private DisabledCard() { }
    private DisabledCard(string cardNumber, string disabilityType, DateOnly issueDate, int patientId, string? cardImagePath = null)
    {
        CardNumber = cardNumber;
        DisabilityType = disabilityType;
        IssueDate = issueDate;
        CardImagePath = cardImagePath;
        PatientId = patientId;
    }

    public static Result<DisabledCard> Create(string cardNumber, string disabilityType, DateOnly issueDate, int patientId, string? cardImagePath)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return DisabledCardErrors.CardNumberIsRequired;
        }
        if (string.IsNullOrWhiteSpace(disabilityType))
        {
            return DisabledCardErrors.DisabilityTypeIsRequired;
        }

        if (issueDate > AlatrafClinicConstants.TodayDate)
        {
            return DisabledCardErrors.IssueDateInvalid;
        }

        if (patientId <= 0)
        {
            return DisabledCardErrors.PatientIdIsRequired;
        }

        return new DisabledCard(cardNumber, disabilityType, issueDate, patientId, cardImagePath);
    }

    public Result<Updated> Update(string cardNumber, string disabilityType, DateOnly issueDate, int patientId, string? cardImagePath)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            return DisabledCardErrors.CardNumberIsRequired;
        }

        if (string.IsNullOrWhiteSpace(disabilityType))
        {
            return DisabledCardErrors.DisabilityTypeIsRequired;
        }

        if (issueDate > AlatrafClinicConstants.TodayDate)
        {
            return DisabledCardErrors.IssueDateInvalid;
        }

        if (patientId <= 0)
        {
            return DisabledCardErrors.PatientIdIsRequired;
        }
        
        CardNumber = cardNumber;
        IssueDate = issueDate;
        CardImagePath = cardImagePath;
        PatientId = patientId;
        DisabilityType = disabilityType;

        return Result.Updated;
    }
}