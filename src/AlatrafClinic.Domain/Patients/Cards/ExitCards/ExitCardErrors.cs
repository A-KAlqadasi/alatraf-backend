using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Patients.Cards.ExitCards;

public static class ExitCardErrors
{
    public static readonly Error PatientIdIsRequired = Error.Validation("ExitCard.PatientIdIsRequired", "Patient Id is required.");
    public static readonly Error RepairCardIdIsRequired = Error.Validation("ExitCard.RepairCardIdIsRequired", "Repair Card Id is required.");
    public static readonly Error SaleIdIsRequired = Error.Validation("ExitCard.SaleIdIsRequired", "Sale Id is required.");
}