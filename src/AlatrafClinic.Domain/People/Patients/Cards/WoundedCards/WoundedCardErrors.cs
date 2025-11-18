using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Patients.Cards.WoundedCards;

public static class WoundedCardErrors
{
    public static readonly Error CardNumberIsRequired = Error.Validation(
        "WoundedCard.CardNumberIsRequired",
        "Card number is required."
    );

    public static readonly Error CardIsExpired = Error.Validation(
        "WoundedCard.CardIsExpired",
        "The card is expired."
    );

    public static readonly Error PatientIdInvalid = Error.Validation(
        "WoundedCard.PatientIdInvalid",
        "Patient Id is invalid."
    );
    public static readonly Error CardNumberDuplicated = Error.Conflict(
        "WoundedCard.CardNumberDuplicated",
        "Card number is already exists."
    );
    public static readonly Error WoundedCardNotFound = Error.NotFound(
        "WoundedCard.WoundedCardNotFound",
        "Wounded card not found."
    );
}