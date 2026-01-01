using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.DisabledCards;

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

    public static readonly Error CardIsExpired = Error.Validation(
        "DisabledCard.CardIsExpired",
        "Card is expired!"
    );

    public static readonly Error PatientIdIsRequired = Error.Validation(
        "DisabledCard.PatientIdIsRequired",
        "Patient Id is required."
    );
    public static readonly Error CardNumberDuplicated = Error.Conflict("DisabledCard.CardNumberDuplicate", "Card number is already exists!");
    public static readonly Error DisabledCardNotFound = Error.NotFound("DisabledCard.DisabledCardNotFound", "Disabled card not found!");
    public static readonly Error IssueDateInvalid = Error.Validation(
        "DisabledCard.IssueDateInvalid",
        "Issue date cannot be in the future."
    );
    public static readonly Error IssueAfterExpiration = Error.Validation(
        "DisabledCard.IssueAfterExpiration",
        "Issue date cannot be after or equal to expiration date."
    );
    public static readonly Error DisabilityTypeIsRequired = Error.Validation(
        "DisabledCard.DisabilityTypeIsRequired",
        "Disability type is required."
    );
}