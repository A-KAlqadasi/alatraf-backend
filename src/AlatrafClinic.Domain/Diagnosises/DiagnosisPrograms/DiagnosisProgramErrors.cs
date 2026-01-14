using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Diagnosises.DiagnosisPrograms;

public static class DiagnosisProgramErrors
{
    public static readonly Error InvalidDiagnosisId = Error.Validation(
        "DiagnosisProgram.InvalidDiagnosisId",
        "رقم التشخيص غير صالح");
    public static readonly Error InvalidMedicalProgramId = Error.Validation(
        "DiagnosisProgram.InvalidMedicalProgramId",
        "رقم برنامج العلاج غير صالح");

    public static readonly Error InvalidDuration = Error.Validation(
        "DiagnosisProgram.InvalidDuration",
        "المدة المقدمة لبرنامج العلاج غير صالحة");

    public static readonly Error NotesTooLong = Error.Validation(
        "DiagnosisProgram.NotesTooLong",
        "الملاحظات الخاصة ببرنامج العلاج تتجاوز الطول المسموح به");
}