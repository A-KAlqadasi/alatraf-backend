using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.TherapyCards;

public static class TherapyCardErrors
{
    public static readonly Error TherapyCardIdInvalid =
    Error.Validation("TherapyCard.TherapyCardIdInvalid", "Therapy Card Id is invalid");
    public static readonly Error DiagnosisIdInvalid =
    Error.Validation("TherapyCard.DiagnosisIdInvalid", "Diagnosis Id is invalid");
    public static readonly Error ProgramStartDateNotInPast =
    Error.Conflict("TherapyCard.ProgramStartDateNotInPast", "Program Start Date must not be in past");
    public static readonly Error NumberOfSessionsIsRequired = Error.Validation("TherapyCard.NumberOfSessionsIsRequired", "Number of sessions is required");
    public static readonly Error TherapyCardTypeInvalid = Error.Validation("TherapyCard.TypeInvalid", "Therapy Card Type invalid");
    public static readonly Error SessionPricePerTypeInvalid = Error.Validation("TherapyCard.SessionPricePerTypeIInvalid", "Session price per type is invalid");
    public static Error InvalidTiming => Error.Conflict(
        code: "TherapyCard.InvalidTiming",
        description: "End time must be after start time.");
    public static Error Readonly => Error.Conflict(
        code: "TherapyCard.Readonly",
        description: "TherapyCard is read-only.");
    public static Error ProgramEnded = Error.Forbidden("TherapyCard.ProgramEnded", "Therapy Card program ended");
    public static Error IsNotPaid = Error.Forbidden("TherapyCard.IsNotPaid", "Therapy Card is not paid");

}