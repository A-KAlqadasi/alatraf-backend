using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Diagnosises.DiagnosisPrograms;

public static class DiagnosisProgramErrors
{
    public static readonly Error DiagnosisIdIsRequired = Error.Validation(
        "DiagnosisProgram.DiagnosisIdIsRequired",
        "The diagnosis Id is required.");
    public static readonly Error MedicalProgramIdIsRequired = Error.Validation(
        "DiagnosisProgram.MedicalProgramIdIsRequired",
        "The medical program Id is required.");

    public static readonly Error InvalidDuration = Error.Validation(
        "DiagnosisProgram.InvalidDuration",
        "The duration provided for the diagnosis program is invalid.");

    public static readonly Error NotesTooLong = Error.Validation(
        "DiagnosisProgram.NotesTooLong",
        "The notes for the diagnosis program exceed the maximum allowed length.");
    public static readonly Error TherapyCardIdIsRequired = Error.Validation(
        "DiagnosisProgram.TherapyCardIdIsRequired",
        "The therapy card Id is required.");
}