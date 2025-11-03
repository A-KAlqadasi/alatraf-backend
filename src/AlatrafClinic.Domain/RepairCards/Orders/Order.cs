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

    private Order(int sectionId, int storeId, OrderType type, int? repairCardId)
    {
        SectionId = sectionId;
        StoreId = storeId;
        RepairCardId = repairCardId;
        OrderType = type;
        Status = OrderStatus.New;
    }

    // ---------- Factory ----------
    public static Result<Order> CreateForRaw(int sectionId, int storeId)
    {
        if (sectionId <= 0) return OrderErrors.InvalidSection;
        if (storeId <= 0) return OrderErrors.InvalidStore;

        var order = new Order(sectionId, storeId, OrderType.Raw, null);
        return order;
    }

    public static Result<Order> CreateForRepairCard(int sectionId, int storeId, int repairCardId)
    {
        if (sectionId <= 0) return OrderErrors.InvalidSection;
        if (repairCardId <= 0) return OrderErrors.InvalidRepairCard;
        if (storeId <= 0) return OrderErrors.InvalidStore;

        var order = new Order(sectionId, storeId, OrderType.RepairCard, repairCardId);
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

    public Result<Updated> UpsertItems(List<OrderItem> newItems)
    {
        if (!IsEditable) return OrderErrors.ReadOnly;
        var list = (newItems ?? Enumerable.Empty<OrderItem>()).ToList();
        if (list.Count == 0) return OrderErrors.NoItems;
        if (list.Any(i => i.StoreItemUnit is null || i.StoreItemUnit.StoreId != this.StoreId))
            return OrderErrors.MixedStores;


        _orderItems.RemoveAll(existing => list.All(v => v.Id != existing.Id));

        foreach (var incoming in list)
        {
            var existing = _orderItems.FirstOrDefault(v => v.Id == incoming.Id);
            if (existing is null)
            {
                _orderItems.Add(incoming);
            }
            else
            {
                var result = existing.Update(this.Id, incoming.StoreItemUnitId, incoming.Quantity, incoming.Price);

                if (result.IsError)
                    return result.Errors;
            }
        }

        return Result.Updated;
    }
    
    public Result<Updated> ReplaceItems(List<(StoreItemUnit storeItemUnit, decimal quantity, decimal price)> newItems)
    {
        if (!IsEditable) return OrderErrors.ReadOnly;
        var list = newItems?.ToList() ?? new();
        if (list.Count == 0) return OrderErrors.NoItems;
        if (list.Any(i => i.storeItemUnit.StoreId != StoreId))
            return OrderErrors.MixedStores;

        _orderItems.Clear();
        foreach (var (storeItemUnit, quantity, price) in list)
        {
            var itemResult = OrderItem.Create(this.Id, storeItemUnit.Id, quantity, price);
            if (itemResult.IsError)
                return itemResult.Errors;

            _orderItems.Add(itemResult.Value);
        }

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

        var exchangeOrderResult = ExchangeOrder.Create(Store.Id);
        if (exchangeOrderResult.IsError)
            return exchangeOrderResult.Errors;

        // create exchange order lines from order items
        var exchangeOrderItems = _orderItems
            .Select(i => ExchangeOrderItem.Create(exchangeOrderResult.Value.Id, i.StoreItemUnit.Id, i.Quantity).Value)
            .ToList();

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