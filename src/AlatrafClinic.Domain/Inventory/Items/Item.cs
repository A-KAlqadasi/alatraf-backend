using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Units;
using AlatrafClinic.Domain.RepairCards.Orders;

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

    public ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();
    public decimal TotalQuantity => _itemUnits.Sum(iu => iu.ToBaseQuantity());

    private Item() { }

    private Item(string name,
                 Unit baseUnit,
                 List<ItemUnit> itemUnits,
                 string? description = null)
    {
        Name = name;
        BaseUnit = baseUnit;
        BaseUnitId = baseUnit.Id;
        _itemUnits = itemUnits;
        Description = description;
    }

    public static Result<Item> Create(string name, Unit baseUnit, List<ItemUnit> itemUnits, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ItemErrors.NameIsRequired;

        if (baseUnit == null)
            return ItemErrors.BaseUnitIsRequired;

        if (itemUnits == null || itemUnits.Count == 0)
            return ItemErrors.ItemUnitsAreRequired;

        return new Item(name, baseUnit, itemUnits, description);
    }

    public Result<Updated> Update(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ItemErrors.NameIsRequired;
        }

        Name = name;
        Description = description;
        return Result.Updated;
    }

    public Result<Updated> UpsertItemUnits(List<ItemUnit> itemUnits)
    {
        _itemUnits.RemoveAll(existing => itemUnits.All(v => v.Id != existing.Id));

        foreach (var incoming in itemUnits)
        {
            var existing = _itemUnits.FirstOrDefault(v => v.Id == incoming.Id);
            if (existing is null)
            {
                _itemUnits.Add(incoming);
            }
            else
            {
                var result = existing.Update(
                    incoming.UnitId,
                    incoming.Price,
                    incoming.ConversionFactor,
                    incoming.MinPriceToPay,
                    incoming.MaxPriceToPay,
                    incoming.Quantity
                );

                if (result.IsError)
                    return result.Errors;
            }
        }

        return Result.Updated;
    }

    public Result<Updated> Deactivate()
    {
        IsActive = false;
        return Result.Updated;
    }
}