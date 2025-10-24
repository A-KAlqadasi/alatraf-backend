using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.TherapyCards.MedicalPrograms;

public static class MedicalProgramErrors
{
    public static readonly Error NameIsRequired = Error.Validation("MedicalProgram.NameIsRequired", "The name of the medical program is required.");
    
}