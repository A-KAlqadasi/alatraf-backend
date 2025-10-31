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
    public static readonly Error DiagnosisProgramAdditionOnlyForTherapyDiagnosis =
        Error.Conflict(
            code: "Diagnosis.DiagnosisProgramAdditionOnlyForTherapyDiagnosis",
            description: "Adding diagnosis programs is only allowed for therapy diagnoses.");
    public static readonly Error TherapyCardAdditionOnlyForTherapyDiagnosis =
        Error.Conflict(
            code: "Diagnosis.TherapyCardAdditionOnlyForTherapyDiagnosis",
            description: "Adding therapy cards is only allowed for therapy diagnoses.");
    public static readonly Error TherapyCardAlreadyAssigned =
        Error.Validation(
            code: "Diagnosis.TherapyCardAlreadyAssigned",
            description: "A therapy card has already been assigned to this diagnosis.");
    public static readonly Error IndustrialPartAdditionOnlyForLimbsDiagnosis = Error.Conflict("Diagnosis.IndustrialPartAdditionOnlyForLimbsDiagnosis", "Adding industrial parts is only allowed for limbs diagnoses.");

    public static readonly Error IndustrialPartsAreRequired = Error.Validation("Diagnosis.IndustrialPartsAreRequired", "Industrial parts are required for this diagnosis.");

    public static readonly Error MedicalProgramsAreRequired = Error.Validation("Diagnosis.MedicalProgramsAreRequired", "Medical programs are required for this diagnosis.");
    public static readonly Error RepairCardAlreadyAssigned =
        Error.Validation(
            code: "Diagnosis.RepairCardAlreadyAssigned",
            description: "A repair card has already been assigned to this diagnosis.");
    public static readonly Error RepairCardAdditionOnlyForLimbsDiagnosis =
        Error.Conflict(
            code: "Diagnosis.RepairCardAdditionOnlyForLimbsDiagnosis",
            description: "Adding repair cards is only allowed for limbs diagnoses.");
    public static readonly Error SaleAlreadyAssigned =
        Error.Validation(
            code: "Diagnosis.SaleAlreadyAssigned",
            description: "A sale has already been assigned to this diagnosis.");
    public static readonly Error SaleAssignmentOnlyForSalesDiagnosis =
        Error.Conflict(
            code: "Diagnosis.SaleAssignmentOnlyForSalesDiagnosis",
            description: "Assigning a sale is only allowed for sales diagnoses.");
}