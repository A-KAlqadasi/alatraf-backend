using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Inventory.ExchangeOrders;
using AlatrafClinic.Domain.Inventory.Stores;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.Sales.Enums;
using AlatrafClinic.Domain.Sales.SalesItems;
namespace AlatrafClinic.Domain.Sales;

public class Sale : AuditableEntity<int>
{
    public SaleStatus Status { get; private set; } = SaleStatus.Draft;

    public int DiagnosisId { get; private set; }
    public Diagnosis Diagnosis { get; private set; } = default!;

    public int StoreId { get; private set; }
    public Store Store { get; set; } = default!;

    public Payment? Payment { get; set; }
    public int? PaymentId { get; private set; }
    public int? ExitCardId { get; private set; }

    private readonly List<SaleItem> _items = new();
    public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();

    public ExchangeOrder? ExchangeOrder { get; private set; }
    public int? ExchangeOrderId { get; private set; }

    public decimal Total => _items.Sum(i => i.Total);

    private Sale() { }

    private Sale(int diagnosisId, int storeId)
    {
        DiagnosisId = diagnosisId;
        StoreId = storeId;
    }

    public static Result<Sale> Create(int diagnosisId, int storeId)
    {
        if (diagnosisId <= 0)
        {
            return SaleErrors.InvalidDiagnosisId;
        }

        if (storeId <= 0) return SaleErrors.StoreRequired;

        return new Sale(diagnosisId, storeId);
    }
    public Result<Updated> UpsertItems(List<SaleItem> newItems)
    {
        var list = (newItems ?? Enumerable.Empty<SaleItem>()).ToList();
        if (list.Count == 0) return SaleErrors.NoItemsProvided;
        if (list.Any(i => i.StoreItemUnit is null || i.StoreItemUnit.StoreId != this.StoreId))
            return SaleErrors.WrongStore;
        
        
        _items.RemoveAll(existing => list.All(v => v.Id != existing.Id));

        foreach (var incoming in list)
        {
            var existing = _items.FirstOrDefault(v => v.Id == incoming.Id);
            if (existing is null)
            {
                _items.Add(incoming);
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

    // ---------- Behavior ------
    public Result<Updated> AssignPayment(Payment payment)
    {
        if (Status != SaleStatus.Draft) return SaleErrors.NotDraft;
        if (payment is null)            return SaleErrors.InvalidPayment;
        Payment = payment;
        PaymentId = payment.Id;
        return Result.Updated;
    }

    public Result<Updated> AddItem(SaleItem item)
    {
        if (Status != SaleStatus.Draft) return SaleErrors.NotDraft;
        if (item is null)               return SaleErrors.InvalidSaleItem;
        if (item.StoreItemUnit.StoreId != StoreId) return SaleErrors.WrongStore;

        var existing = _items.FirstOrDefault(i => i.StoreItemUnitId == item.StoreItemUnitId);
        if (existing is not null)
        {
            existing.IncreaseQuantity(item.Quantity);
            existing.Update(this.Id, existing.StoreItemUnitId, existing.Quantity, item.Price);
            return Result.Updated;
        }

        item.AssignSale(this);
        
        _items.Add(item);
        return Result.Updated;
    }

    // ---------- State transitions ----------
    /// <summary>
    /// Payment confirmed -> stock decreases -> ExchangeOrder created.
    /// </summary>
    public Result<Updated> Post(string exchangeOrderNumber)
    {
        if (Status == SaleStatus.Posted)    return SaleErrors.AlreadyPosted;
        if (Status == SaleStatus.Cancelled) return SaleErrors.AlreadyCancelled;

        if (Payment is null) return SaleErrors.PaymentRequired;
        if (_items.Count == 0)   return SaleErrors.NoItemsProvided;

        // Create ExchangeOrder entity 
        var exchangeOrderResult = ExchangeOrder.Create(Store.Id);

        if (exchangeOrderResult.IsError) return exchangeOrderResult.Errors;

        // Build ExchangeOrderItems from sale items
        var orderItems = _items
            .Select(i => ExchangeOrderItem.Create(exchangeOrderResult.Value.Id, i.StoreItemUnit.Id, i.Quantity).Value)
            .ToList();

        var upsertResult = exchangeOrderResult.Value.UpsertItems(orderItems);    

        var exchangeOrder = exchangeOrderResult.Value;

        // Assign number (provided by app/service)
        exchangeOrder.AssignSale(this, exchangeOrderNumber);

        // Approve (decrease stock)
        var approveResult = exchangeOrder.Approve();
        
        if (approveResult.IsError)
            return approveResult.Errors;

        ExchangeOrder = exchangeOrder;
        ExchangeOrderId = exchangeOrder.Id;
        Status = SaleStatus.Posted;

        return Result.Updated;
    }

    public Result<Updated> Cancel()
    {
        if (Status == SaleStatus.Cancelled) return SaleErrors.AlreadyCancelled;
        if (Status == SaleStatus.Posted) return SaleErrors.AlreadyPosted;
        Status = SaleStatus.Cancelled;
        return Result.Updated;
    }

    public Result<Updated> AssignExitCard(int exitCardId)
    {
        if (exitCardId <= 0) return SaleErrors.InvalidExitCardId;
        ExitCardId = exitCardId;
        return Result.Updated;
    }
}