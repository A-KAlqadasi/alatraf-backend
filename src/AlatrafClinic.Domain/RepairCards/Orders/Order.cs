using System.Reflection.Metadata;

using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.ExchangeOrders;
using AlatrafClinic.Domain.Inventory.Stores;
using AlatrafClinic.Domain.Organization.Departments;
using AlatrafClinic.Domain.Organization.Sections;
using AlatrafClinic.Domain.RepairCards.Enums;

namespace AlatrafClinic.Domain.RepairCards.Orders;
public class Order : AuditableEntity<int>
{
    public int? RepairCardId { get; private set; }
    public RepairCard? RepairCard { get; private set; }

    public int SectionId { get; private set; }
    public Section Section { get; private set; } = default!;

    public OrderType OrderType { get; private set; } = OrderType.Raw;
    public OrderStatus Status { get; private set; } = OrderStatus.New;

    public int StoreId { get; private set; }
    public Store Store { get; private set; } = default!;

    public int? ExchangeOrderId { get; private set; }
    public ExchangeOrder? ExchangeOrder { get; private set; }

    private readonly List<OrderItem> _orderItems = new();
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    private Order() { }

    private Order(int sectionId, Store store, List<OrderItem> items, OrderType type, int? repairCardId)
    {
        SectionId = sectionId;
        Store = store;
        StoreId = store.Id;
        RepairCardId = repairCardId;
        OrderType = type;
        Status = OrderStatus.New;
        _orderItems = items;
    }

    // ---------- Factory ----------
    public static Result<Order> CreateForRaw(int sectionId, Store store, List<OrderItem> items)
    {
        if (sectionId <= 0) return OrderErrors.InvalidSection;
        if (store is null) return OrderErrors.InvalidStore;
        if (items == null || !items.Any()) return OrderErrors.NoItems;

        var order = new Order(sectionId, store, items, OrderType.Raw, null);
        return order;
    }

    public static Result<Order> CreateForRepairCard(int sectionId, Store store, int repairCardId, List<OrderItem> items)
    {
        if (sectionId <= 0) return OrderErrors.InvalidSection;
        if (repairCardId <= 0) return OrderErrors.InvalidRepairCard;
        if (store is null) return OrderErrors.InvalidStore;
        if (items == null || !items.Any()) return OrderErrors.NoItems;

        var order = new Order(sectionId, store, items, OrderType.RepairCard, repairCardId);
        return order;
    }

    public bool IsEditable => Status == OrderStatus.New;

    // ---------- Behavior ----------
    public Result<Updated> UpdateSection(int sectionId)
    {
        if (!IsEditable) return OrderErrors.ReadOnly;
        if (sectionId <= 0) return OrderErrors.InvalidSection;

        SectionId = sectionId;
        return Result.Updated;
    }

    public Result<Updated> ReplaceItems(List<OrderItem> newItems)
    {
        if (!IsEditable) return OrderErrors.ReadOnly;
        var list = newItems?.ToList() ?? new();
        if (list.Count == 0) return OrderErrors.NoItems;
        if (list.Any(i => i.StoreItemUnit.StoreId != StoreId))
            return OrderErrors.MixedStores;

        _orderItems.Clear();
        _orderItems.AddRange(list);
        return Result.Updated;
    }

    public Result<Updated> Cancel()
    {
        if (!IsEditable) return OrderErrors.ReadOnly;
        Status = OrderStatus.Cancelled;
        return Result.Updated;
    }

    // ---------- Exchange Order (approval) ----------
    public Result<Updated> Approve(string exchangeOrderNumber)
    {
        if (!IsEditable) return OrderErrors.ReadOnly;
        if (_orderItems.Count == 0) return OrderErrors.NoItems;

        // create exchange order lines from order items
        var exchangeOrderItems = _orderItems
            .Select(i => ExchangeOrderItem.Create(i.StoreItemUnit, i.Quantity).Value)
            .ToList();

        var exchangeOrderResult = ExchangeOrder.Create(Store, exchangeOrderItems);
        if (exchangeOrderResult.IsError)
            return exchangeOrderResult.Errors;

        var exchangeOrder = exchangeOrderResult.Value;

        exchangeOrder.AssignOrder(this, exchangeOrderNumber);

        // approve exchange order (decrease stock)
        var approval = exchangeOrder.Approve();
        if (approval.IsError)
            return approval.Errors;

        ExchangeOrder = exchangeOrder;
        ExchangeOrderId = exchangeOrder.Id;
        Status = OrderStatus.Completed;
        return Result.Updated;
    }
}