using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Units;

namespace AlatrafClinic.Domain.RepairCards.IndustrialParts;

public class IndustrialPartUnit : AuditableEntity<int>
{
    public int IndustrialPartId { get; private set; }
    public IndustrialPart IndustrialPart { get; set; } = default!;
    public int UnitId { get; private set; }
    public Unit? Unit { get; set; }
    public decimal PricePerUnit { get; private set; }

    private IndustrialPartUnit() { }
    private IndustrialPartUnit(int industrialPartId, int unitId, decimal price)
    {
        IndustrialPartId = industrialPartId;
        UnitId = unitId;
        PricePerUnit = price;
    }

    public static Result<IndustrialPartUnit> Create(int industrialPartId, int unitId, decimal price)
    {
        if (industrialPartId <= 0)
        {
            return IndustrialPartUnitErrors.IndustrialPartIdInvalid;
        }
        if (unitId <= 0)
        {
            return IndustrialPartUnitErrors.UnitIdInvalid;
        }
        if (price <= 0)
        {
            return IndustrialPartUnitErrors.PriceInvalid;
        }

        return new IndustrialPartUnit(industrialPartId, unitId, price);
    }

    public Result<Updated> Update(int industrialPartId, int unitId, decimal price)
    {
        if (industrialPartId <= 0)
        {
            return IndustrialPartUnitErrors.IndustrialPartIdInvalid;
        }

        if (unitId <= 0)
        {
            return IndustrialPartUnitErrors.UnitIdInvalid;
        }
        if (price <= 0)
        {
            return IndustrialPartUnitErrors.PriceInvalid;
        }
        IndustrialPartId = industrialPartId;
        UnitId = unitId;
        PricePerUnit = price;
        return Result.Updated;
    }

}