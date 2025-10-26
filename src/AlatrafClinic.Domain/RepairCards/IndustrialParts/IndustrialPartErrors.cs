using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.RepairCards.IndustrialParts;

public static class IndustrialPartErrors
{
    public static readonly Error NameIsRequired = Error.Validation("IndustrialPart.NameIsRequired", "Industrial part name is required");
    public static readonly Error UnitAlreadyExists = Error.Validation("IndustrialPart.UnitAlreadyExists", "Industrial part unit already exists");

}