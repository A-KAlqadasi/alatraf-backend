using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Stores;
using AlatrafClinic.Domain.RepairCards.Orders;
using AlatrafClinic.Domain.Sales;

namespace AlatrafClinic.Domain.Inventory.ExchangeOrders;

public class ExchangeOrder : AuditableEntity<int>
{
    public string Number { get; private set; } = string.Empty;   // e.g., "EX-2025-001"

    public bool IsApproved { get; private set; }
    public string? Notes { get; private set; }

    public int? SaleId { get; private set; }
    public Sale? Sale { get; private set; }

    public int? OrderId { get; private set; }
    public Order? Order { get; set; }

    // store reference (which store released the items)
    public int StoreId { get; private set; }
    public Store Store { get; set; } = default!;

    private readonly List<ExchangeOrderItem> _items = new();
    public IReadOnlyCollection<ExchangeOrderItem> Items => _items.AsReadOnly();

    private ExchangeOrder() { }

    private ExchangeOrder(int storeId, string? notes = null)
    {
        StoreId = storeId;
        Notes = notes;
        IsApproved = false;
    }

    public static Result<ExchangeOrder> Create(int storeId, string? notes = null)
    {
        if (storeId <= 0)
            return ExchangeOrderErrors.StoreRequired;

        return new ExchangeOrder(storeId, notes);
    }

    public Result<Updated> UpsertItems(List<ExchangeOrderItem> items)
    {
        if (IsApproved)
            return ExchangeOrderErrors.AlreadyApproved;

        _items.RemoveAll(existing => items.All(v => v.Id != existing.Id));

        foreach (var incoming in items)
        {
            var existing = _items.FirstOrDefault(v => v.Id == incoming.Id);
            if (existing is null)
            {
                _items.Add(incoming);
            }
            else
            {
                var result = existing.Update(this.Id, incoming.StoreItemUnitId, incoming.Quantity);

                if (result.IsError)
                    return result.Errors;
            }
        }

        return Result.Updated;
    }

    public Result<Updated> Approve()
    {
        if (IsApproved)
            return ExchangeOrderErrors.AlreadyApproved;

        // decrease stock
        foreach (var line in _items)
        {
            var dec = line.StoreItemUnit.Decrease(line.Quantity);
            if (dec.IsError) return dec.Errors;
        }

        IsApproved = true;
        return Result.Updated;
    }
    public Result<Updated> AssignOrder(Order order, string number)
    {
        if (IsApproved)
        {
            return ExchangeOrderErrors.AlreadyApproved;
        }

        if (order is null)
        {
            return ExchangeOrderErrors.OrderIsRequired;
        }
        Order = order;
        OrderId = order.Id;
        Number = number;
        return Result.Updated;
    }
    public Result<Updated> AssignSale(Sale sale, string number)
    {
        if (IsApproved)
        {
            return ExchangeOrderErrors.AlreadyApproved;
        }

        if (sale is null)
        {
            return ExchangeOrderErrors.SaleIsRequired;
        }
        Sale = sale;
        SaleId = sale.Id;
        Number = number;
        return Result.Updated;
    }
}