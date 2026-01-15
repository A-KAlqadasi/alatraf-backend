using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.IndustrialParts;

public static class IndustrialPartErrors
{
    public static readonly Error NameIsRequired = Error.Validation("IndustrialPart.NameIsRequired", "اسم الطرف الصناعي مطلوب");
    public static readonly Error UnitAlreadyExists = Error.Conflict("IndustrialPart.UnitAlreadyExists", "وحدة الطرف الصناعي موجودة بالفعل");
    public static readonly Error NameAlreadyExists = Error.Conflict("IndustrialPart.NameAlreadyExists", "اسم الطرف الصناعي موجود بالفعل");
    public static readonly Error IndustrialPartNotFound = Error.NotFound("IndustrialPart.NotFound", "الطرف الصناعي غير موجود");

}