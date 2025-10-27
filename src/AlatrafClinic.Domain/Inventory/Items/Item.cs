using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards.Orders;

namespace AlatrafClinic.Domain.Inventory.Items;

public class Item : AuditableEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? TotalQuantity => ItemUnits.Sum(iu => iu.Quantity ?? 0);
    public bool? IsActive { get; set; }
    private readonly List<ItemUnit> _itemUnits = new();
    public IReadOnlyCollection<ItemUnit> ItemUnits => _itemUnits.AsReadOnly();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    // public ICollection<StoreItems> StoreItems { get; set; } = new();
    // public ICollection<PurchaseItem> PurchaseItems { get; set; } = new();
    // public ICollection<SalesItems> SalesItems { get; set; } = new();

    private Item()
    {
    }
    public Item(string name, List<ItemUnit> itemUnits, string? description = null)
    {
        Name = name;
        _itemUnits = itemUnits;
        Description = description;
        IsActive = true;
    }
    public static Result<Item> Create(string name, List<ItemUnit> itemUnits, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ItemErrors.NameIsRequired;
        }
        if (itemUnits == null || itemUnits.Count == 0)
        {
            return ItemErrors.ItemUnitsAreRequired;
        }

        return new Item(name, itemUnits, description);
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
                var updateItemUnitResult = existing.Update(incoming.UnitId, incoming.Price, incoming.MinPriceToPay, incoming.MaxPriceToPay, incoming.Quantity);

                if (updateItemUnitResult.IsError)
                {
                    return updateItemUnitResult.Errors;
                }
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