using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Units;

namespace AlatrafClinic.Domain.Inventory.Items;

public class ItemUnit : AuditableEntity<int>
{
    public int ItemId { get; set; }
    public Item? Item { get; set; }
    public int UnitId { get; set; }
    public Unit? Unit { get; set; }
    public decimal Price { get; set; }
    public decimal? MinPriceToPay { get; set; }
    public decimal? MaxPriceToPay { get; set; }
    public decimal? Quantity { get; set; }
    private ItemUnit()
    {
    }
    public ItemUnit(int unitId, decimal price, decimal? minPriceToPay = null, decimal? maxPriceToPay = null, decimal? quantity = null)
    {
        UnitId = unitId;
        Price = price;
        MinPriceToPay = minPriceToPay;
        MaxPriceToPay = maxPriceToPay;
        Quantity = quantity;
    }

    public static Result<ItemUnit> Create(int unitId, decimal price, decimal? minPriceToPay = null, decimal? maxPriceToPay = null, decimal? quantity = null)
    {
        return new ItemUnit(unitId, price, minPriceToPay, maxPriceToPay, quantity);
    }

    public Result<Updated> Update(int unitId, decimal price, decimal? minPriceToPay = null, decimal? maxPriceToPay = null, decimal? quantity = null)
    {

        UnitId = unitId;
        Price = price;
        MinPriceToPay = minPriceToPay;
        MaxPriceToPay = maxPriceToPay;
        Quantity = quantity;
        return Result.Updated;
    }

    public Result<Updated> Increase(decimal quantity)
    {
        if (quantity <= 0)
        {
            return ItemUnitErrors.InvalidQuantity;
        }
        if (quantity > Quantity)
        {
            return ItemUnitErrors.NotEnoughQuantity;
        }
        
        Quantity += quantity;
        return Result.Updated;
    }
    public Result<Updated> Decrease(decimal quantity)
    {
        if (quantity <= 0)
        {
            return ItemUnitErrors.InvalidQuantity;
        }
        
        if (Quantity <= 0 || Quantity < quantity)
        {
            return ItemUnitErrors.NotEnoughQuantity;
        }
        
        Quantity -= quantity;
        return Result.Updated;
    }
}