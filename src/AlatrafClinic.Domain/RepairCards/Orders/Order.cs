using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.Sections;
using AlatrafClinic.Domain.Inventory.ExchangeOrders;
using AlatrafClinic.Domain.Inventory.Items;
using AlatrafClinic.Domain.Inventory.Stores;
using AlatrafClinic.Domain.RepairCards.Enums;

namespace AlatrafClinic.Domain.RepairCards.Orders;

public class Order : AuditableEntity<int>
{
    public int? RepairCardId { get; private set; }
    public RepairCard? RepairCard { get; private set; }

    public int SectionId { get; private set; }
    public Section Section { get; private set; } = default!;

    public OrderType OrderType { get; private set; } = OrderType.Raw;
    public OrderStatus Status { get; private set; } = OrderStatus.Draft;

    private readonly List<OrderItem> _orderItems = new();
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    private Order() { }

    private Order(int sectionId, OrderType type, int? repairCardId)
    {
        SectionId = sectionId;
        RepairCardId = repairCardId;
        OrderType = type;
        Status = OrderStatus.Draft;
    }

    // ---------- Factory ----------
    public static Result<Order> CreateForRaw(int sectionId)
    {
        if (sectionId <= 0) return OrderErrors.InvalidSection;

        var order = new Order(sectionId, OrderType.Raw, null);
        return order;
    }

    public static Result<Order> CreateForRepairCard(int sectionId, int repairCardId)
    {
        if (sectionId <= 0) return OrderErrors.InvalidSection;
        if (repairCardId <= 0) return OrderErrors.InvalidRepairCard;

        var order = new Order(sectionId, OrderType.RepairCard, repairCardId);
        return order;
    }

    public bool IsEditable => Status == OrderStatus.Draft;

    // ---------- Behavior ----------
    public Result<Updated> UpdateSection(int sectionId)
    {
        if (!IsEditable) return OrderErrors.ReadOnly;
        if (sectionId <= 0) return OrderErrors.InvalidSection;

        SectionId = sectionId;
        return Result.Updated;
    }

    public Result<Updated> UpsertItems(List<(ItemUnit itemUnit, decimal quantity)> newItems)
    {
        if (!IsEditable) return OrderErrors.ReadOnly;

        if (newItems is null || newItems.Count == 0) return OrderErrors.NoItems;

        _orderItems.RemoveAll(existing => newItems.All(v => v.itemUnit.Id != existing.ItemUnitId));

        foreach (var incoming in newItems)
        {
            var existing = _orderItems.FirstOrDefault(v => v.ItemUnitId == incoming.itemUnit.Id);
            if (existing is null)
            {
                var itemResult = OrderItem.Create(this.Id, incoming.itemUnit, incoming.quantity);
                if (itemResult.IsError)
                {
                    return itemResult.Errors;
                }
                _orderItems.Add(itemResult.Value);
            }
            else
            {
                var result = existing.Update(this.Id, incoming.itemUnit, incoming.quantity);

                if (result.IsError)
                {
                    return result.Errors;
                }
            }
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
    public Result<Updated> Approve()
    {
        if (!IsEditable) return OrderErrors.ReadOnly;
        if (_orderItems.Count == 0) return OrderErrors.NoItems;




        Status = OrderStatus.Posted;
        return Result.Updated;
    }
}