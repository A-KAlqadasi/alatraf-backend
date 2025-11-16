using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Stores;
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
    // public decimal Quantity => _storeItemUnits.Sum(itu => itu.Quantity);

    // private readonly List<StoreItemUnit> _storeItemUnits = new();
    // public IReadOnlyCollection<StoreItemUnit> StoreItemUnits => _storeItemUnits.AsReadOnly();

    private ItemUnit() { }

    private ItemUnit(int unitId,
                     decimal price,
                     decimal conversionFactor,
                     decimal? minPriceToPay = null,
                     decimal? maxPriceToPay = null)
    {
        UnitId = unitId;
        Price = price;
        ConversionFactor = conversionFactor <= 0 ? 1 : conversionFactor;
        MinPriceToPay = minPriceToPay;
        MaxPriceToPay = maxPriceToPay;
    }

    public static Result<ItemUnit> Create(
        int unitId,
        decimal price,
        decimal conversionFactor = 1,
        decimal? minPriceToPay = null,
        decimal? maxPriceToPay = null)
    {
        if (unitId <= 0)
            return ItemUnitErrors.UnitRequired;

        if (price < 0)
            return ItemUnitErrors.InvalidPrice;

        if (conversionFactor <= 0)
            return ItemUnitErrors.InvalidConversionFactor;

        return new ItemUnit(unitId, price, conversionFactor, minPriceToPay, maxPriceToPay);
    }

    public Result<Updated> Update(
        int unitId,
        decimal price,
        decimal conversionFactor = 1,
        decimal? minPriceToPay = null,
        decimal? maxPriceToPay = null)
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

        return Result.Updated;
    }

    public decimal ToBaseQuantity(decimal quantity) => quantity * ConversionFactor;
}