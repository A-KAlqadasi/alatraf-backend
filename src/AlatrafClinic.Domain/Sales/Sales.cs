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

public class Sales : AuditableEntity<int>
{
    private readonly List<SalesItem> _salesItems = [];
    public IReadOnlyCollection<SalesItem> SalesItems => _salesItems.AsReadOnly();

    public decimal Total { get; private set; }

    public int DiagnosisId { get; private set; }
    public Diagnosis Diagnosis { get; private set; } = default!;

    public int? PaymentId { get; private set; }
    // public Payment? Payment { get; private set; }

    public int? ExitCardId { get; private set; }
    public ExitCard? ExitCard { get; private set; }

    public int? ExchangeOrderId { get; private set; }
    // public ExchangeOrder? ExchangeOrder { get; private set; }

    private Sales(int diagnosisId)
    {
        DiagnosisId = diagnosisId;
        Total = 0m;
    }

    public static Result<Sales> Create(int diagnosisId)
    {
        if (diagnosisId <= 0)
            return SalesErrors.DiagnosisRequired;

        return new Sales(diagnosisId);
    }

    public Result<Updated> AddSalesItem(SalesItem item)
    {
        if (_salesItems.Any(i => i.ItemId == item.ItemId))
            return SalesErrors.ItemAlreadyExists;

        if (item.Quantity <= 0 || item.Price <= 0)
            return SalesItemErrors.PriceInvalid;

        _salesItems.Add(item);
        RecalculateTotal();

        return Result.Updated;
    }

    public Result<Updated> RemoveItem(int itemId)
    {
        var existing = _salesItems.FirstOrDefault(i => i.ItemId == itemId);
        if (existing is null)
            return SalesErrors.NoItems; 

        _salesItems.Remove(existing);
        RecalculateTotal();

        return Result.Updated;
    }

    public Result<Updated> SetPayment(int paymentId)
    {
        if (paymentId <= 0) return SalesErrors.InvalidPayment;
        if (PaymentId is not null) return SalesErrors.PaymentAlreadyLinked;

        PaymentId = paymentId;
        return Result.Updated;
    }

    public Result<Updated> SetExitCard(int exitCardId)
    {
        if (exitCardId <= 0) return SalesErrors.InvalidExitCard;
        if (ExitCardId is not null) return SalesErrors.ExitCardAlreadyLinked;

        ExitCardId = exitCardId;
        return Result.Updated;
    }

    private void RecalculateTotal() =>
        Total = _salesItems.Sum(i => i.CalculateTotal());
   
}

