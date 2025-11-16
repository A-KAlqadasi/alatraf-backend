using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;
using AlatrafClinic.Domain.RepairCards.IndustrialParts;

namespace AlatrafClinic.Domain.Inventory.Units;

public class Unit : AuditableEntity<int>
{
    public string Name { get; set; } = default!;
    
    public ICollection<ItemUnit> ItemUnits { get; set; } = new List<ItemUnit>();
    public ICollection<IndustrialPartUnit> IndustrialPartUnits { get; set; } = new List<IndustrialPartUnit>();
    private Unit()
    {
    }

    public Unit(string name)
    {
        Name = name;
    }
    public static Result<Unit> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return UnitErrors.NameIsRequired;
        }

        return new Unit(name);
    }
    public Result<Updated> Update(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return UnitErrors.NameIsRequired;
        }
        Name = name;
        return Result.Updated;
    }
}