using System.Reflection.Metadata;

using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Organization.Departments;
using AlatrafClinic.Domain.Organization.Sections;
using AlatrafClinic.Domain.RepairCards.Enums;

namespace AlatrafClinic.Domain.RepairCards.Orders;

public class Order : AuditableEntity<int>
{
    public int? RepairCardId { get; set; }
    public RepairCard? RepairCard { get; set; }
    public int? SectionId { get; set; }
    public Section? Section { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public OrderType OrderType { get; set; }
    public int? ExchangeOrderId { get; set; }

    //public ExchangeOrder? ExchangeOrder { get; set; }
    private readonly List<OrderItem> _orderItems = new();

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    private Order() { }

    private Order(int sectionId, List<OrderItem> orderItems, OrderStatus orderStatus)
    {
        OrderType = OrderType.Raw;
        SectionId = sectionId;
        OrderStatus = orderStatus;
        _orderItems = orderItems;
    }

    public static Result<Order> Create(int sectionId, List<OrderItem> orderItems)
    {
        if (sectionId <= 0)
        {
            return OrderErrors.SectionIdInvalid;
        }

        if (orderItems == null || !orderItems.Any())
        {
            return OrderErrors.OrderItemsAreRequired;
        }

        return new Order(sectionId, orderItems, OrderStatus.New);
    }
    public bool IsEditable => OrderStatus == OrderStatus.New;

    public Result<Updated> Update(int sectionId)
    {
        if (!IsEditable)
        {
            return OrderErrors.ReadOnly;
        }

        if (sectionId <= 0)
        {
            return OrderErrors.SectionIdInvalid;
        }

        SectionId = sectionId;

        return Result.Updated;
    }

    public Result<Updated> UpsertOrderItems(List<OrderItem> incomingOrderItems)
    {
        if (!IsEditable)
        {
            return OrderErrors.ReadOnly;
        }
        _orderItems.RemoveAll(existing => incomingOrderItems.All(v => v.Id != existing.Id));

        foreach (var incoming in incomingOrderItems)
        {
            var existing = _orderItems.FirstOrDefault(v => v.Id == incoming.Id);
            if (existing is null)
            {
                _orderItems.Add(incoming);
            }
            else
            {
                var updateOrderItemResult = existing.Update(incoming.ItemUnitId, incoming.Price, incoming.Quantity);

                if (updateOrderItemResult.IsError)
                {
                    return updateOrderItemResult.Errors;
                }
            }
        }
        return Result.Updated;
    }
    public Result<Updated> MarkAsCompleted()
    {
        if (!IsEditable)
        {
            return OrderErrors.ReadOnly;
        }

        if (ExchangeOrderId is null)
        {
            return OrderErrors.OrderCannotCompleteUntilHasExchangeOrder;
        }

        OrderStatus = OrderStatus.Completed;

        return Result.Updated;
    }
    public Result<Updated> MarkAsCancelled()
    {
        if (!IsEditable)
        {
            return OrderErrors.ReadOnly;
        }
        OrderStatus = OrderStatus.Cancelled;
        return Result.Updated;
    }

    public Result<Updated> LinkExchangeOrder(int exchangeOrderId)
    {
        if (!IsEditable)
        {
            return OrderErrors.ReadOnly;
        }
        
        if(exchangeOrderId <= 0)
        {
            return OrderErrors.InvalidExchangeOrderId;
        }

        ExchangeOrderId = exchangeOrderId;
        OrderStatus = OrderStatus.Completed;

        return Result.Updated;
    }
    
    public Result<Updated> MakeAsRepairCardOrder()
    {
        if (!IsEditable)
        {
            return OrderErrors.ReadOnly;
        }

        OrderType = OrderType.RepairCard;

        return Result.Updated;
    }
}