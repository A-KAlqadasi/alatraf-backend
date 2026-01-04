using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Diagnosises.DiagnosisPrograms;

public static class DiagnosisProgramErrors
{
    public static readonly Error InvalidDiagnosisId = Error.Validation(
        "DiagnosisProgram.InvalidDiagnosisId",
        "The diagnosis Id is invalid.");
    public static readonly Error InvalidMedicalProgramId = Error.Validation(
        "DiagnosisProgram.InvalidMedicalProgramId",
        "The medical program Id is invalid.");

    public static readonly Error InvalidDuration = Error.Validation(
        "DiagnosisProgram.InvalidDuration",
        "The duration provided for the diagnosis program is invalid.");

    public static readonly Error NotesTooLong = Error.Validation(
        "DiagnosisProgram.NotesTooLong",
        "The notes for the diagnosis program exceed the maximum allowed length.");
}