using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;

namespace AlatrafClinic.Domain.Inventory.Stores;

public class Store : AuditableEntity<int>
{
    public string Name { get; private set; } = string.Empty;
    private readonly List<StoreItemUnit> _storeItemUnits = new();
    public IReadOnlyCollection<StoreItemUnit> StoreItemUnits => _storeItemUnits.AsReadOnly();

    private Store() { }

    private Store(string name)
    {
        Name = name;
    }

    public static Result<Store> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return StoreErrors.NameIsRequired;

        return new Store(name);
    }

    public Result<Updated> Update(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return StoreErrors.NameIsRequired;

        Name = name;
        return Result.Updated;
    }

    public Result<Updated> AddItemUnit(ItemUnit itemUnit, decimal quantity)
    {
        if (itemUnit == null)
            return StoreErrors.ItemUnitIsRequired;

        var existing = _storeItemUnits.FirstOrDefault(siu => siu.ItemUnitId == itemUnit.Id);
        if (existing is not null)
        {
            return existing.Increase(quantity);
        }

        var newStoreItem = StoreItemUnit.Create(this, itemUnit, quantity);
        if (newStoreItem.IsError)
            return newStoreItem.Errors;

        _storeItemUnits.Add(newStoreItem.Value);
        return Result.Updated;
    }
    public Result<Updated> AdjustItemUnit(ItemUnit itemUnit, decimal quantityDelta)
    {
        if (itemUnit == null)
            return StoreErrors.ItemUnitIsRequired;

        if (quantityDelta == 0)
            return Result.Updated;

        var existing = _storeItemUnits.FirstOrDefault(siu => siu.ItemUnitId == itemUnit.Id);

        if (existing is not null)
        {
            if (quantityDelta > 0)
            {
                return existing.Increase(quantityDelta);
            }

            var decreaseResult = existing.Decrease(Math.Abs(quantityDelta));
            return decreaseResult;
        }

        // no existing store item
        if (quantityDelta < 0)
        {
            return StoreItemUnitErrors.NotFound;
        }

        var newStoreItem = StoreItemUnit.Create(this, itemUnit, quantityDelta);
        if (newStoreItem.IsError)
            return newStoreItem.Errors;

        _storeItemUnits.Add(newStoreItem.Value);
        return Result.Updated;
    }

    public Result<Deleted> RemoveItemUnit(ItemUnit itemUnit)
    {
        if (itemUnit is null)
            return StoreItemUnitErrors.ItemIsRequired;

        var existing = _storeItemUnits.FirstOrDefault(siu => siu.ItemUnitId == itemUnit.Id);
        if (existing is null)
            return StoreItemUnitErrors.NotFound;

        // TODO: if there are domain rules preventing deletion, check them here and return CannotDelete
        _storeItemUnits.Remove(existing);
        return Result.Deleted;
    }
    public decimal GetTotalQuantity() => _storeItemUnits.Sum(i => i.Quantity);

}

