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
    public static Error InvalidDiagnosisId = Error.Validation("TherapyCard.InvalidDiagnosisId", "Diagnosis Id is invalid");
    public static Error TherapyCardNotFound = Error.NotFound("TherapyCard.NotFound", "Therapy Card not found");
    public static Error DiagnosisNotIncluded = Error.Validation("TherapyCard.DiagnosisNotIncluded", "Diagnosis must be included in the Therapy Card");
    public static Error InvalidCardStatus = Error.Validation("TherapyCard.InvalidCardStatus", "Card Status is invalid");
    public static Error TherapyCardNotExpired = Error.Conflict("TherapyCard.NotExpired", "Therapy Card is not expired");
    public static Error TherapyCardExpired = Error.Conflict("TherapyCard.Expired", "Therapy Card is expired");
    public static Error SessionNotFound = Error.NotFound("TherapyCard.SessionNotFound", "Session not found");
    public static readonly Error PaymentNotFound =
    Error.NotFound("TherapyCard.PaymentNotFound", "Payment for this therapy card not found");
    public static readonly Error NoActiveTherapyCardFound =
    Error.NotFound("TherapyCard.NoActiveTherapyCardFound", "No active therapy card found for the patient");

}