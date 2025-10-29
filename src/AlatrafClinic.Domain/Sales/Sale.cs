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
    public Store Store { get; private set; } = default!;

    public Payment? Payment { get; private set; }
    public int? PaymentId { get; private set; }
    public int? ExitCardId { get; private set; }

    private readonly List<SaleItem> _items = new();
    public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();

    public ExchangeOrder? ExchangeOrder { get; private set; }
    public int? ExchangeOrderId { get; private set; }

    public decimal Total => _items.Sum(i => i.Total);

    private Sale() { }

    private Sale(int diagnosisId, Store store, IEnumerable<SaleItem> items)
    {
        DiagnosisId = diagnosisId;
        Store = store;
        StoreId = store.Id;
        _items = new List<SaleItem>(items);
    }

    public static Result<Sale> Create(int diagnosisId, Store store, IEnumerable<SaleItem> items)
    {
        if (diagnosisId <= 0) return SaleErrors.DiagnosisRequired;
        if (store is null)    return SaleErrors.StoreRequired;

        var list = (items ?? Enumerable.Empty<SaleItem>()).ToList();
        if (list.Count == 0)  return SaleErrors.NoItemsProvided;
        if (list.Any(i => i.StoreItemUnit is null || i.StoreItemUnit.StoreId != store.Id))
            return SaleErrors.WrongStore;

        return new Sale(diagnosisId, store, list);
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
            existing.Update(existing.StoreItemUnit, existing.Quantity, item.Price);
            return Result.Updated;
        }

        typeof(SaleItem).GetProperty(nameof(SaleItem.Sale))!.SetValue(item, this);
        typeof(SaleItem).GetProperty(nameof(SaleItem.SaleId))!.SetValue(item, this.Id);
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

        // Build ExchangeOrderItems from sale items
        var orderItems = _items
            .Select(i => ExchangeOrderItem.Create(i.StoreItemUnit, i.Quantity).Value)
            .ToList();

        // Create ExchangeOrder entity
        var exchangeOrderResult = ExchangeOrder.Create(Store, orderItems);
        if (exchangeOrderResult.IsError) return exchangeOrderResult.Errors;

        var exchangeOrder = exchangeOrderResult.Value;

        // Assign number (provided by app/service)
        typeof(ExchangeOrder).GetProperty(nameof(ExchangeOrder.Number))!
            .SetValue(exchangeOrder, exchangeOrderNumber);
        typeof(ExchangeOrder).GetProperty(nameof(ExchangeOrder.Sale))!
            .SetValue(exchangeOrder, this);
        typeof(ExchangeOrder).GetProperty(nameof(ExchangeOrder.SaleId))!
            .SetValue(exchangeOrder, this.Id);

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
        if (Status == SaleStatus.Posted)    return SaleErrors.AlreadyPosted;
        Status = SaleStatus.Cancelled;
        return Result.Updated;
    }
}