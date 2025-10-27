using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Units;

namespace AlatrafClinic.Domain.Inventory.Items;

public class ItemUnit : AuditableEntity<int>
{
    public int ItemId { get; private set; }
    public Item Item { get; private set; } = default!;
    public int UnitId { get; private set; }
    public Unit Unit { get; private set; } = default!;
    public decimal Price { get; private set; }
    public decimal? MinPriceToPay { get; private set; }
    public decimal? MaxPriceToPay { get; private set; }
    public decimal ConversionFactor { get; private set; } = 1;
    public decimal Quantity { get; private set; } = 0;

    private ItemUnit() { }

    private ItemUnit(int unitId,
                     decimal price,
                     decimal conversionFactor,
                     decimal? minPriceToPay = null,
                     decimal? maxPriceToPay = null,
                     decimal? quantity = null)
    {
        UnitId = unitId;
        Price = price;
        ConversionFactor = conversionFactor <= 0 ? 1 : conversionFactor;
        MinPriceToPay = minPriceToPay;
        MaxPriceToPay = maxPriceToPay;
        Quantity = quantity ?? 0;
    }

    public static Result<ItemUnit> Create(
        int unitId,
        decimal price,
        decimal conversionFactor = 1,
        decimal? minPriceToPay = null,
        decimal? maxPriceToPay = null,
        decimal? quantity = null)
    {
        if (unitId <= 0)
            return ItemUnitErrors.UnitRequired;

        if (price < 0)
            return ItemUnitErrors.InvalidPrice;

        if (conversionFactor <= 0)
            return ItemUnitErrors.InvalidConversionFactor;

        return new ItemUnit(unitId, price, conversionFactor, minPriceToPay, maxPriceToPay, quantity);
    }

    public Result<Updated> Update(
        int unitId,
        decimal price,
        decimal conversionFactor = 1,
        decimal? minPriceToPay = null,
        decimal? maxPriceToPay = null,
        decimal? quantity = null)
    {
        if (unitId <= 0)
            return ItemUnitErrors.UnitRequired;

        if (price < 0)
            return ItemUnitErrors.InvalidPrice;

        UnitId = unitId;
        Price = price;
        ConversionFactor = conversionFactor <= 0 ? 1 : conversionFactor;
        MinPriceToPay = minPriceToPay;
        MaxPriceToPay = maxPriceToPay;
        Quantity = quantity ?? Quantity;

        return Result.Updated;
    }

    public Result<Updated> Increase(decimal quantity)
    {
        if (quantity <= 0)
            return ItemUnitErrors.InvalidQuantity;

        Quantity += quantity;
        return Result.Updated;
    }

    public Result<Updated> Decrease(decimal quantity)
    {
        if (quantity <= 0)
            return ItemUnitErrors.InvalidQuantity;

        if (Quantity < quantity)
            return ItemUnitErrors.NotEnoughQuantity;

        Quantity -= quantity;
        return Result.Updated;
    }

    public decimal ToBaseQuantity() => Quantity * ConversionFactor;
}