using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Patients.Cards.WoundedCards;

public static class WoundedCardErrors
{
    public static readonly Error CardNumberIsRequired = Error.Validation(
        "WoundedCard.CardNumberIsRequired",
        "Card number is required."
    );

    public static readonly Error ExpirationIsRequired = Error.Validation(
        "WoundedCard.ExpirationIsRequired",
        "Expiration date is required."
    );

    public static readonly Error ExpirationMustBeInTheFuture = Error.Validation(
        "WoundedCard.ExpirationMustBeInTheFuture",
        "Expiration date must be in the future."
    );

    public static readonly Error PatientIdIsRequired = Error.Validation(
        "WoundedCard.PatientIdIsRequired",
        "Patient Id is required."
    );
}