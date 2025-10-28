using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Patients.Cards.ExitCards;
using AlatrafClinic.Domain.Sales.SalesItems;
namespace AlatrafClinic.Domain.Sales;

public class Sale : AuditableEntity<int>
{
    public decimal Total => _salesItems.Sum(i => i.Total);
    public bool IsActive { get; private set; } = true;

    public int DiagnosisId { get; private set; }
    public Diagnosis Diagnosis { get; private set; } = default!;

    public int? PaymentId { get; private set; }
    // public Payment? Payment { get; private set; }

    public int? ExchangeOrderId { get; private set; }
    // public ExchangeOrder? ExchangeOrder { get; private set; }

    public int? ExitCardId { get; private set; }
    public ExitCard? ExitCard { get; private set; }

    private readonly List<SalesItem> _salesItems = new();
    public IReadOnlyCollection<SalesItem> SalesItems => _salesItems.AsReadOnly();

    private Sale() { }

    private Sale(int diagnosisId, List<SalesItem> salesItems,
                 int? paymentId = null, int? exchangeOrderId = null, int? exitCardId = null)
    {
        DiagnosisId = diagnosisId;
        _salesItems = salesItems;
        PaymentId = paymentId;
        ExchangeOrderId = exchangeOrderId;
        ExitCardId = exitCardId;
    }

    // ---------------- FACTORY ----------------
    public static Result<Sale> Create(int diagnosisId, List<SalesItem> salesItems,
                                      int? paymentId = null,
                                      int? exchangeOrderId = null,
                                      int? exitCardId = null)
    {
        if (diagnosisId <= 0)
            return SaleErrors.DiagnosisRequired;

        if (salesItems is null || salesItems.Count == 0)
            return SaleErrors.NoItemsProvided;

        return new Sale(diagnosisId, salesItems, paymentId, exchangeOrderId, exitCardId);
    }

    // ---------------- BEHAVIOR ----------------
    public Result<Updated> AddSalesItem(SalesItem salesItem)
    {
        if (!IsActive)
            return SaleErrors.NotEditable;

        if (ExchangeOrderId.HasValue || ExitCardId.HasValue)
            return SaleErrors.LockedBySourceDocument;

        if (salesItem is null)
            return SaleErrors.InvalidSalesItem;

        var existing = _salesItems
            .FirstOrDefault(i => i.ItemId == salesItem.ItemId && i.UnitId == salesItem.UnitId);

        if (existing is not null)
        {
            var result = existing.Update(existing.Quantity + salesItem.Quantity, salesItem.Price);
            if (result.IsError)
                return result.Errors;
        }
        else
        {
            _salesItems.Add(salesItem);
        }

        return Result.Updated;
    }

    public Result<Updated> RemoveSalesItem(int salesItemId)
    {
        if (!IsActive)
            return SaleErrors.NotEditable;

        if (ExchangeOrderId.HasValue || ExitCardId.HasValue)
            return SaleErrors.LockedBySourceDocument;

        var item = _salesItems.FirstOrDefault(x => x.Id == salesItemId);
        if (item is null)
            return SaleErrors.SalesItemNotFound;

        _salesItems.Remove(item);
        return Result.Updated;
    }

    public Result<Updated> AssignPayment(int paymentId)
    {
        if (!IsActive)
            return SaleErrors.NotEditable;

        if (paymentId <= 0)
            return SaleErrors.InvalidPayment;

        PaymentId = paymentId;
        return Result.Updated;
    }

    public Result<Updated> AssignExchangeOrder(int exchangeOrderId)
    {
        if (!IsActive)
            return SaleErrors.NotEditable;

        if (ExitCardId.HasValue)
            return SaleErrors.ExitCardConflictWithExchangeOrder;

        if (ExchangeOrderId.HasValue)
            return SaleErrors.ExchangeOrderAlreadyAssigned;

        if (exchangeOrderId <= 0)
            return SaleErrors.InvalidExchangeOrder;

        ExchangeOrderId = exchangeOrderId;
        return Result.Updated;
    }

    public Result<Updated> AssignExitCard(int exitCardId)
    {
        if (!IsActive)
            return SaleErrors.NotEditable;

        if (ExchangeOrderId.HasValue)
            return SaleErrors.ExitCardConflictWithExchangeOrder;

        if (ExitCardId.HasValue)
            return SaleErrors.ExitCardAlreadyAssigned;

        if (exitCardId <= 0)
            return SaleErrors.InvalidExitCard;

        ExitCardId = exitCardId;
        return Result.Updated;
    }

    public Result<Updated> Deactivate()
    {
        if (!IsActive)
            return SaleErrors.AlreadyInactive;

        IsActive = false;
        return Result.Updated;
    }
}