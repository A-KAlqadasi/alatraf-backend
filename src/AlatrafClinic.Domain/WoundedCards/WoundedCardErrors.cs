using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.WoundedCards;

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
    public static readonly Error IssueDateInvalid = Error.Validation(
        "WoundedCard.IssueDateInvalid",
        "Issue date cannot be in the future."
    );
    public static readonly Error IssueAfterExpiration = Error.Validation(
        "WoundedCard.IssueAfterExpiration",
        "Issue date cannot be after or equal to expiration date."
    );
    
}