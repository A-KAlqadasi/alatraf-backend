using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Patients.Cards.DisabledCards;

public static class DisabledCardErrors
{
    public static readonly Error CardNumberIsRequired = Error.Validation(
        "DisabledCard.CardNumberIsRequired",
        "Card number is required."
    );

    public static readonly Error ExpirationIsRequired = Error.Validation(
        "DisabledCard.ExpirationIsRequired",
        "Expiration date is required."
    );

    public static readonly Error ExpirationMustBeInTheFuture = Error.Validation(
        "DisabledCard.ExpirationMustBeInTheFuture",
        "Expiration date must be in the future."
    );

    public static readonly Error PatientIdIsRequired = Error.Validation(
        "DisabledCard.PatientIdIsRequired",
        "Patient Id is required."
    );

}