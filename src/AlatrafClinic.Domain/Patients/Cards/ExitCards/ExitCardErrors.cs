using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Patients.Cards.ExitCards;

public static class ExitCardErrors
{
    public static readonly Error PatientIdIsRequired = Error.Validation("ExitCard.PatientIdIsRequired", "Patient Id is required.");
}