using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.RepairCards.IndustrialParts;

public static class IndustrialPartUnitErrors
{
    public static readonly Error UnitIdInvalid = Error.Validation("IndustrialPartUnit.UnitIdInvalid", "Unit Id is invalid");

    public static readonly Error PriceInvalid = Error.Validation("IndustrialPartUnit.PriceInvalid", "Price is invalid");
    public static readonly Error IndustrialPartIdInvalid = Error.Validation("IndustrialPartUnit.IndustrialPartIdInvalid", "Industrial Part Id is invalid");
}