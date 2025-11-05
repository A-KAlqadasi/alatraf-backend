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
    public static readonly Error InjuryReasonIsRequired =
        Error.Validation(
            code: "Diagnosis.InjuryReasonIsRequired",
            description: "Injury reason is required.");
    public static readonly Error InjurySideIsRequired =
        Error.Validation(
            code: "Diagnosis.InjurySideIsRequired",
            description: "Injury side is required.");
    public static readonly Error InjuryTypeIsRequired =
        Error.Validation(
            code: "Diagnosis.InjuryTypeIsRequired",
            description: "Injury type is required.");
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
    public static readonly Error DiagnosisProgramAdditionOnlyForTherapyDiagnosis =
        Error.Conflict(
            code: "Diagnosis.DiagnosisProgramAdditionOnlyForTherapyDiagnosis",
            description: "Adding diagnosis programs is only allowed for therapy diagnoses.");
    public static readonly Error IndustrialPartAdditionOnlyForLimbsDiagnosis = Error.Conflict("Diagnosis.IndustrialPartAdditionOnlyForLimbsDiagnosis", "Adding industrial parts is only allowed for limbs diagnoses.");

    public static readonly Error IndustrialPartsAreRequired = Error.Validation("Diagnosis.IndustrialPartsAreRequired", "Industrial parts are required for this diagnosis.");

    public static readonly Error MedicalProgramsAreRequired = Error.Validation("Diagnosis.MedicalProgramsAreRequired", "Medical programs are required for this diagnosis.");
    public static readonly Error DiagnosisNotFound =
        Error.NotFound(
            code: "Diagnosis.DiagnosisNotFound",
            description: "Diagnosis not found.");
}