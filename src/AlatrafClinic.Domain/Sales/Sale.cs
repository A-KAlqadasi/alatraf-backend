using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.ExitCards;
using AlatrafClinic.Domain.Inventory.ExchangeOrders;
using AlatrafClinic.Domain.Inventory.Items;
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

    public bool IsPaid => Diagnosis.Payments.Any(p => p.DiagnosisId == DiagnosisId && p.IsCompleted);
    public Payment? Payment => Diagnosis.Payments.FirstOrDefault(p => p.DiagnosisId == DiagnosisId);

    public ExitCard? ExitCard { get; private set; }

    private readonly List<SaleItem> _saleItems = new();
    public IReadOnlyCollection<SaleItem> SaleItems => _saleItems.AsReadOnly();

    public ExchangeOrder? ExchangeOrder { get; private set; }
    public decimal Total => _saleItems.Sum(i => i.Total);
    public string? Notes { get; set; }

    private Sale() { }

    private Sale(int diagnosisId, string? notes = null)
    {
        DiagnosisId = diagnosisId;
        Notes = notes;
    }

    public static Result<Sale> Create(int diagnosisId, string? notes = null)
    {
        if (diagnosisId <= 0)
        {
            return SaleErrors.InvalidDiagnosisId;
        }

        return new Sale(diagnosisId, notes);
    }

    public Result<Updated> UpsertItems(List<(ItemUnit itemUnit, decimal quantity)> newItems)
    {
        if (Status != SaleStatus.Draft) return SaleErrors.NotDraft;

        if (newItems is null || newItems.Count == 0) return SaleErrors.NoItems;

        _saleItems.RemoveAll(existing => newItems.All(v => v.itemUnit.Id != existing.ItemUnitId));

        foreach (var (itemUnit, quantity) in newItems)
        {
            var existing = _saleItems.FirstOrDefault(v => v.ItemUnitId == itemUnit.Id);
            if (existing is null)
            {
                var itemResult = SaleItem.Create(this.Id, itemUnit, quantity);
                if (itemResult.IsError)
                {
                    return itemResult.Errors;
                }

                _saleItems.Add(itemResult.Value);
            }
            else
            {
                var result = existing.Update(this.Id, itemUnit, quantity);

                if (result.IsError)
                {
                    return result.Errors;
                }
            }
        }

        return Result.Updated;
    }

    public Result<Updated> Post(string exchangeOrderNumber, List<(StoreItemUnit StoreItemUnit, decimal Quantity)> items, string? notes = null)
    {
        if (Status == SaleStatus.Posted) return SaleErrors.AlreadyPosted;
        if (Status == SaleStatus.Cancelled) return SaleErrors.AlreadyCancelled;

        if (!IsPaid) return SaleErrors.PaymentRequired;
        if (_saleItems.Count == 0) return SaleErrors.NoItems;

        if (string.IsNullOrWhiteSpace(exchangeOrderNumber))
        {
            return SaleErrors.ExchangeOrderRequired;
        }

        if (items.Count != _saleItems.Count)
        {
            return SaleErrors.ItemsConflictInOrderAndExchangeOrder;
        }

        foreach (var saleItem in _saleItems)
        {
            var matchingItem = items.FirstOrDefault(i => i.StoreItemUnit.ItemUnitId == saleItem.ItemUnitId);

            if (matchingItem.StoreItemUnit is null)
            {
                return SaleErrors.ItemsConflictInOrderAndExchangeOrder;
            }

            if (matchingItem.Quantity != saleItem.Quantity)
            {
                return SaleErrors.ItemsConflictInOrderAndExchangeOrder;
            }
            if (matchingItem.Quantity > matchingItem.StoreItemUnit.Quantity)
            {
                return SaleErrors.QuantityExceedsAvailable;
            }
        }

        var exchangeOrderResult = ExchangeOrder.Create(items.FirstOrDefault().StoreItemUnit.StoreId, notes);

        if (exchangeOrderResult.IsError)
        {
            return exchangeOrderResult.Errors;
        }
        var exchangeOrder = exchangeOrderResult.Value;

        // create exchange order lines from sale items and set StoreItemUnit navigation
        var exchangeOrderItems = items
            .Select(i => {
                var created = ExchangeOrderItem.Create(exchangeOrder.Id, i.StoreItemUnit.Id, i.Quantity).Value;
                created.StoreItemUnit = i.StoreItemUnit;
                return created;
            })
            .ToList();
        var upsertResult = exchangeOrder.UpsertItems(exchangeOrderItems);
        if (upsertResult.IsError)
        {
            return upsertResult.Errors;
        }

        var assignResult = exchangeOrder.AssignSale(this.Id, exchangeOrderNumber);
        if (assignResult.IsError)
            return assignResult.Errors;

        // approve exchange order (decrease stock)
        var approval = exchangeOrder.Approve();

        if (approval.IsError)
            return approval.Errors;

        ExchangeOrder = exchangeOrder;
        Status = SaleStatus.Posted;
        return Result.Updated;
    }

 public Result<Updated> MarkPosted()
    {
        if (Status == SaleStatus.Posted) return SaleErrors.AlreadyPosted;
        if (Status == SaleStatus.Cancelled) return SaleErrors.AlreadyCancelled;
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

    public Result<Updated> AssignExitCard(string? notes)
    {
        if (ExitCard is not null)
        {
            return SaleErrors.ExitCardAlreadyAssigned;
        }
        var patientId = Diagnosis.PatientId;

        var exitCardResult = ExitCard.Create(patientId, notes);
        if (exitCardResult.IsError)
        {
            return exitCardResult.Errors;
        }

        ExitCard = exitCardResult.Value;
        ExitCard.AssignSale(this);

        return Result.Updated;
    }

}