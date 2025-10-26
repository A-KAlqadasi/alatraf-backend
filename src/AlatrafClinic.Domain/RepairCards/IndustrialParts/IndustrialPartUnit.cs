using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Units;

namespace AlatrafClinic.Domain.RepairCards.IndustrialParts;

public class IndustrialPartUnit : AuditableEntity<int>
{
    public int IndustrialPartId { get; set; }
    public IndustrialPart? IndustrialPart { get; set; }
    public int UnitId { get; set; }
    public Unit? Unit { get; set; }
    public decimal PricePerUnit { get; set; }

    private IndustrialPartUnit() { }
    private IndustrialPartUnit(int unitId, decimal price)
    {
        UnitId = unitId;
        PricePerUnit = price;
    }

    public static Result<IndustrialPartUnit> Create(int unitId, decimal price)
    {
        if (unitId <= 0)
        {
            return IndustrialPartUnitErrors.UnitIdInvalid;
        }
        if (price <= 0)
        {
            return IndustrialPartUnitErrors.PriceInvalid;
        }

        return new IndustrialPartUnit(unitId, price);
    }

    public Result<Updated> Update(int unitId, decimal price)
    {
        if (unitId <= 0)
        {
            return IndustrialPartUnitErrors.UnitIdInvalid;
        }
        if (price <= 0)
        {
            return IndustrialPartUnitErrors.PriceInvalid;
        }
        UnitId = unitId;
        PricePerUnit = price;
        return Result.Updated;
    }

}