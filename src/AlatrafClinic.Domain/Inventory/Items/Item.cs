using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Units;

namespace AlatrafClinic.Domain.Inventory.Items;

public class Item : AuditableEntity<int>
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    public int BaseUnitId { get; private set; }
    public Unit BaseUnit { get; private set; } = default!;

    private readonly List<ItemUnit> _itemUnits = new();
    public IReadOnlyCollection<ItemUnit> ItemUnits => _itemUnits.AsReadOnly();

    // public decimal TotalQuantity => _itemUnits.Sum(iu => iu.ToBaseQuantity());

    private Item() { }

    private Item(string name, Unit baseUnit, string? description = null)
    {
        Name = name;
        BaseUnit = baseUnit;
        BaseUnitId = baseUnit.Id;
        Description = description;


    }

    public static Result<Item> Create(string name, Unit baseUnit, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ItemErrors.NameIsRequired;

        if (baseUnit == null)
            return ItemErrors.BaseUnitIsRequired;

        return new Item(name, baseUnit, description);
    }

    public Result<Updated> Update(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ItemErrors.NameIsRequired;

        Name = name;
        Description = description;


        return Result.Updated;
    }


    public Result<Updated> AddOrUpdateItemUnit(
        int unitId,
        decimal price,
        decimal conversionFactor = 1,
        decimal? minPriceToPay = null,
        decimal? maxPriceToPay = null)
    {
        var existing = _itemUnits.FirstOrDefault(u => u.UnitId == unitId);

        if (existing is null)
        {
            var create = ItemUnit.Create(unitId, price, conversionFactor, minPriceToPay, maxPriceToPay);
            if (create.IsError)
                return create.Errors;

            _itemUnits.Add(create.Value);

        }
        else
        {
            var update = existing.Update(unitId, price, conversionFactor, minPriceToPay, maxPriceToPay);
            if (update.IsError)
                return update.Errors;


        }

        return Result.Updated;
    }
    public Result<Updated> RemoveItemUnit(int unitId)
    {
        var unit = _itemUnits.FirstOrDefault(u => u.UnitId == unitId);
        if (unit == null)
            return ItemUnitErrors.UnitRequired;

        _itemUnits.Remove(unit);

        return Result.Updated;
    }


    public Result<Updated> Deactivate()
    {
        if (!IsActive)
            return ItemErrors.AlreadyInactive;

        IsActive = false;

        return Result.Updated;
    }

    public Result<Updated> Activate()
    {
        if (IsActive)
            return Result.Updated;

        IsActive = true;

        return Result.Updated;
    }
}
