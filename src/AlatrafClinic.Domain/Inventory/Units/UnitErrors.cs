using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Inventory.Units;

public static class UnitErrors
{
    public static readonly Error NameIsRequired = Error.Validation("Unit.NameIsRequired", "Unit name is required.");
    public static readonly Error UnitNotFound = Error.NotFound("Unit.NotFound", "The specified unit was not found.");
}