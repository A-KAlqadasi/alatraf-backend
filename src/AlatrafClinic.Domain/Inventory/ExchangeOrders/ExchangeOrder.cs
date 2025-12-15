using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Stores;

namespace AlatrafClinic.Domain.Inventory.ExchangeOrders;

public class ExchangeOrder : AuditableEntity<int>
{
    public string Number { get; private set; } = string.Empty;
    public bool IsApproved { get; private set; }
    public string? Notes { get; private set; }

    public int? RelatedOrderId { get; private set; }
    public int? RelatedSaleId { get; private set; }

    public int StoreId { get; private set; }
    public Store Store { get; set; } = default!;

    private readonly List<ExchangeOrderItem> _items = new();
    public IReadOnlyCollection<ExchangeOrderItem> Items => _items.AsReadOnly();

    private ExchangeOrder() { }

    private ExchangeOrder(int storeId, string number, string? notes)
    {
        StoreId = storeId;
        Number = number;
        Notes = notes;
    }

    // ================================================================
    //   FACTORY: Create exchange order for SALE
    // ================================================================
    public static Result<ExchangeOrder> CreateForSale(
        int saleId,
        int storeId,
        string number,
        List<ExchangeOrderItem> items,
        string? notes)
    {
        if (saleId <= 0)
            return ExchangeOrderErrors.SaleRequired;

        if (storeId <= 0)
            return ExchangeOrderErrors.StoreRequired;

        if (string.IsNullOrWhiteSpace(number))
            return ExchangeOrderErrors.ExchangeOrderNumberRequired;

        var order = new ExchangeOrder(storeId, number, notes)
        {
            RelatedSaleId = saleId
        };

        foreach (var item in items)
            order.AddItem(item);

        return order;
    }

    // ================================================================
    //   FACTORY: Create exchange order for WORK ORDER
    // ================================================================
    public static Result<ExchangeOrder> CreateForOrder(
        int orderId,
        int storeId,
        string number,
        List<ExchangeOrderItem> items,
        string? notes)
    {
        if (orderId <= 0)
            return ExchangeOrderErrors.OrderRequired;

        if (storeId <= 0)
            return ExchangeOrderErrors.StoreRequired;

        if (string.IsNullOrWhiteSpace(number))
            return ExchangeOrderErrors.ExchangeOrderNumberRequired;

        var order = new ExchangeOrder(storeId, number, notes)
        {
            RelatedOrderId = orderId
        };

        foreach (var item in items)
            order.AddItem(item);

        return order;
    }

    // ================================================================
    //     BEHAVIOR: Add an item to the exchange order
    // ================================================================
    public Result<Updated> AddItem(ExchangeOrderItem item)
    {
        if (IsApproved)
            return ExchangeOrderErrors.AlreadyApproved;

        _items.Add(item);
        return Result.Updated;
    }

    // ================================================================
    //     BEHAVIOR: Replace all items (UI editing case)
    // ================================================================
    public Result<Updated> ReplaceItems(List<ExchangeOrderItem> items)
    {
        if (IsApproved)
            return ExchangeOrderErrors.AlreadyApproved;

        _items.Clear();
        _items.AddRange(items);

        return Result.Updated;
    }

    // ================================================================
    //     BEHAVIOR: Approve
    // ================================================================
    public Result<Updated> Approve()
    {
        if (IsApproved)
            return ExchangeOrderErrors.AlreadyApproved;

        IsApproved = true;
        return Result.Updated;
    }
}
