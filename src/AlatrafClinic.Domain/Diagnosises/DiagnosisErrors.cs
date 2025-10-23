using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Diagnosises;

public static class DiagnosisErrors
{
    public static readonly Error DiagnosisTextIsRequired =
        Error.Validation(
            code: "Diagnosis.DiagnosisTextIsRequired",
            description: "Diagnosis text is required.");
    public static readonly Error InvalidInjuryDate =
        Error.Validation(
            code: "Diagnosis.InvalidInjuryDate",
            description: "Injury date is invalid.");
    public static readonly Error InvalidReasonId =
        Error.Validation(
            code: "Diagnosis.InvalidReasonId",
            description: "Reason ID is invalid.");
    public static readonly Error InvalidSideId =
        Error.Validation(
            code: "Diagnosis.InvalidSideId",
            description: "Side ID is invalid.");
    public static readonly Error InvalidTypeId =
        Error.Validation(
            code: "Diagnosis.InvalidTypeId",
            description: "Type ID is invalid.");
    public static readonly Error InvalidTicketId =
        Error.Validation(
            code: "Diagnosis.InvalidTicketId",
            description: "Ticket ID is invalid.");
    public static readonly Error InvalidPatientId =
        Error.Validation(
            code: "Diagnosis.InvalidPatientId",
            description: "Patient ID is invalid.");
    public static readonly Error InvalidDiagnosisType =
        Error.Validation(
            code: "Diagnosis.InvalidDiagnosisType",
            description: "Diagnosis type is invalid.");
}