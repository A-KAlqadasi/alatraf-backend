using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.IndustrialParts;
using AlatrafClinic.Domain.Inventory.Items;

namespace AlatrafClinic.Domain.Inventory.Units;

public class GeneralUnit : AuditableEntity<int>
{
    public string Name { get; set; } = default!;
    
    public ICollection<ItemUnit> ItemUnits { get; set; } = new List<ItemUnit>();
    public ICollection<IndustrialPartUnit> IndustrialPartUnits { get; set; } = new List<IndustrialPartUnit>();
    private GeneralUnit()
    {
    }

    public GeneralUnit(string name)
    {
        Name = name;
    }
    public static Result<GeneralUnit> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return UnitErrors.NameIsRequired;
        }

        return new GeneralUnit(name);
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