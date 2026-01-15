using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.MedicalPrograms;

public static class MedicalProgramErrors
{
    public static readonly Error NameIsRequired = Error.Validation("MedicalProgram.NameIsRequired", "اسم البرنامج العلاجي مطلوب.");
    public static readonly Error MedicalProgramNotFound = Error.Validation("MedicalProgram.NotFound", "البرنامج العلاجي غير موجود.");
    public static readonly Error NameAlreadyExists = Error.Conflict("MedicalProgram.NameExists", "اسم البرنامج العلاجي موجود بالفعل.");
    
}