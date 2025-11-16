using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.RepairCards.IndustrialParts;

public static class IndustrialPartErrors
{
    public static readonly Error NameIsRequired = Error.Validation("IndustrialPart.NameIsRequired", "Industrial part name is required");
    public static readonly Error UnitAlreadyExists = Error.Validation("IndustrialPart.UnitAlreadyExists", "Industrial part unit already exists");
    public static readonly Error NameAlreadyExists = Error.Conflict("IndustrialPart.NameAlreadyExists", "Industrial part name already exists");
    public static readonly Error IndustrialPartNotFound = Error.NotFound("IndustrialPart.NotFound", "Industrial part not found");
    public static readonly Error NoIndustrialPartsFound = Error.NotFound("IndustrialPart.NoIndustrialPartsFound", "No industrial parts found");

}