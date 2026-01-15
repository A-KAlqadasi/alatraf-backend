using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.IndustrialParts;

public static class IndustrialPartUnitErrors
{
    public static readonly Error UnitIdInvalid = Error.Validation("IndustrialPartUnit.UnitIdInvalid", "معرف الوحدة غير صالح");

    public static readonly Error PriceInvalid = Error.Validation("IndustrialPartUnit.PriceInvalid", "السعر غير صالح");
    public static readonly Error IndustrialPartIdInvalid = Error.Validation("IndustrialPartUnit.IndustrialPartIdInvalid", "معرف الطرف الصناعي غير صالح");
    public static readonly Error IndustrialPartUnitNotFound = Error.NotFound("IndustrialPartUnit.NotFound", "وحدة الطرف الصناعي غير موجود");
}