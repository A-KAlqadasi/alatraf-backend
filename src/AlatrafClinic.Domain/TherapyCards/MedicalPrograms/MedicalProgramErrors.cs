using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.TherapyCards.MedicalPrograms;

public static class MedicalProgramErrors
{
    public static readonly Error NameIsRequired = Error.Validation("MedicalProgram.NameIsRequired", "The name of the medical program is required.");
    public static readonly Error MedicalProgramNotFound = Error.Validation("MedicalProgram.NotFound", "The medical program was not found.");
    
}