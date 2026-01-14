using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Diagnosises;

public static class DiagnosisErrors
{
    public static readonly Error DiagnosisTextIsRequired =
        Error.Validation(
            code: "Diagnosis.DiagnosisTextIsRequired",
            description: "نص التشخيص مطلوب");
    public static readonly Error InvalidInjuryDate =
        Error.Validation(
            code: "Diagnosis.InvalidInjuryDate",
            description: "تاريخ الإصابة غير صالح");
    public static readonly Error InjuryReasonIsRequired =
        Error.Validation(
            code: "Diagnosis.InjuryReasonIsRequired",
            description: "سبب الإصابة مطلوب");
    public static readonly Error InjurySideIsRequired =
        Error.Validation(
            code: "Diagnosis.InjurySideIsRequired",
            description: "جانب الإصابة مطلوب");
    public static readonly Error InjuryTypeIsRequired =
        Error.Validation(
            code: "Diagnosis.InjuryTypeIsRequired",
            description: "نوع الإصابة مطلوب");
    public static readonly Error InvalidTicketId =
        Error.Validation(
            code: "Diagnosis.InvalidTicketId",
            description: "رقم التذكرة غير صالح");
    public static readonly Error InvalidPatientId =
        Error.Validation(
            code: "Diagnosis.InvalidPatientId",
            description: "رقم المريض غير صالح");
    public static readonly Error InvalidDiagnosisType =
        Error.Validation(
            code: "Diagnosis.InvalidDiagnosisType",
            description: "نوع التشخيص غير صالح");
    public static readonly Error DiagnosisProgramAdditionOnlyForTherapyDiagnosis =
        Error.Conflict(
            code: "Diagnosis.DiagnosisProgramAdditionOnlyForTherapyDiagnosis",
            description: "إضافة برامج العلاج مسموح بها فقط لتشخيصات العلاج");
    public static readonly Error IndustrialPartAdditionOnlyForLimbsDiagnosis = Error.Conflict("Diagnosis.IndustrialPartAdditionOnlyForLimbsDiagnosis", "إضافة الأجزاء الصناعية مسموح بها فقط لتشخيصات الأطراف");

    public static readonly Error IndustrialPartsAreRequired = Error.Validation("Diagnosis.IndustrialPartsAreRequired", "الاطراف الصناعية مطلوبة لهذا التشخيص");

    public static readonly Error MedicalProgramsAreRequired = Error.Validation("Diagnosis.MedicalProgramsAreRequired", "برامج العلاج مطلوبة لهذا التشخيص");
    public static readonly Error DiagnosisNotFound =
        Error.NotFound(
            code: "Diagnosis.DiagnosisNotFound",
            description: "التشخيص غير موجود");
    public static readonly Error SaleIsRequired = Error.Validation("Diagnosis.SaleIsRequired", "المبيعات مطلوبة لتعيين التشخيص للمبيعات");
    public static readonly Error SaleAssignOnlyForSaleDiagnosis = Error.Conflict("Diagnosis.SaleAssignOnlyForSaleDiagnosis", "تعيين المبيعات فقط لتشخيصات المبيعات");
    public static readonly Error PaymentIsRequired = Error.Validation("Diagnosis.PaymentIsRequired", "الدفع مطلوب لتعيين التشخيص للمدفوعات");
    public static readonly Error TherpyCardAssignmentOnlyForTherapyDiagnosis = Error.Conflict("Diagnosis.TherpyCardAssignmentOnlyForTherapyDiagnosis", "تعيين كرت العلاج فقط لتشخيصات العلاج");
    public static readonly Error TherapyCardIsRequired = Error.Validation("Diagnosis.TherapyCardIsRequired", "كرت العلاج مطلوب للتشخيص");
    public static readonly Error RepairCardAssignmentOnlyForLimbsDiagnosis = Error.Conflict("Diagnosis.RepairCardAssignmentOnlyForLimbsDiagnosis", "تعيين كرت الإصلاح فقط لتشخيصات الأطراف");
    public static readonly Error RepairCardIsRequired = Error.Validation("Diagnosis.RepairCardIsRequired", "كرت الإصلاح مطلوب للتشخيص");

}