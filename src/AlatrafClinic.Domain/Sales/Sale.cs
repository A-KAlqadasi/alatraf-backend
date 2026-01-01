using System;

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

public class Sale : AuditableEntity<int>, IAggregateRoot
{
    public SaleStatus Status { get; private set; } = SaleStatus.Draft;
    public Guid? SagaId { get; private set; }
    public bool InventoryReservationCompleted { get; private set; }
    public bool PaymentRecorded { get; private set; }

    public int DiagnosisId { get; private set; }
    public Diagnosis Diagnosis { get; private set; } = default!;

    // public bool IsPaid => Diagnosis.Payments.Any(p => p.DiagnosisId == DiagnosisId && p.IsCompleted);
    public Payment? Payment => Diagnosis.Payments.FirstOrDefault(p => p.DiagnosisId == DiagnosisId);

    public ExitCard? ExitCard { get; private set; }

    private readonly List<SaleItem> _saleItems = new();
    public IReadOnlyCollection<SaleItem> SaleItems => _saleItems.AsReadOnly();

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
        // if (diagnosisId <= 0)
        //     return SaleErrors.InvalidDiagnosisId;

        return new Sale(diagnosisId, notes);
    }

    public Result<Updated> AttachSaga(Guid sagaId)
    {
        if (sagaId == Guid.Empty) return SaleErrors.InvalidSagaId;

        if (SagaId is null)
        {
            SagaId = sagaId;
            return Result.Updated;
        }

        if (SagaId == sagaId) return Result.Updated;

        return SaleErrors.SagaMismatch;
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
                if (itemResult.IsError) return itemResult.Errors;

                _saleItems.Add(itemResult.Value);
            }
            else
            {
                var result = existing.Update(this.Id, itemUnit, quantity);
                if (result.IsError) return result.Errors;
            }
        }

        return Result.Updated;
    }

    public Result<Updated> MarkInventoryReserved(Guid sagaId)
    {
        // For legacy flows without saga, allow empty and keep SagaId null
        if (sagaId == Guid.Empty && SagaId is null)
        {
            SagaId = null;
        }
        if (Status != SaleStatus.Draft) return SaleErrors.NotDraft;

        if (SagaId is null)
        {
            SagaId = sagaId;
        }
        else if (SagaId != sagaId)
        {
            return SaleErrors.SagaMismatch;
        }

        InventoryReservationCompleted = true;
        return Result.Updated;
    }

    public Result<Updated> Confirm(Guid sagaId)
    {
        if (sagaId == Guid.Empty && SagaId is null)
        {
            SagaId = null;
        }
        if (Status == SaleStatus.Canceled) return SaleErrors.AlreadyCancelled;
        if (Status == SaleStatus.Confirmed) return Result.Updated;
        if (!InventoryReservationCompleted) return SaleErrors.InventoryNotReserved;

        if (SagaId is null)
        {
            SagaId = sagaId;
        }
        else if (SagaId != sagaId)
        {
            return SaleErrors.SagaMismatch;
        }

        Status = SaleStatus.Confirmed;
        return Result.Updated;
    }

    public Result<Updated> MarkPaymentCreated(Guid sagaId)
    {
        if (sagaId == Guid.Empty && SagaId is null)
        {
            SagaId = null;
        }
        if (PaymentRecorded && SagaId == sagaId)
        {
            return Result.Updated;
        }
        if (PaymentRecorded && SagaId != sagaId)
        {
            return SaleErrors.SagaMismatch;
        }
        if (!InventoryReservationCompleted)
        {
            return SaleErrors.InventoryNotReserved;
        }
        if (Status != SaleStatus.Confirmed)
        {
            return SaleErrors.NotConfirmed;
        }
        if (SagaId is null)
        {
            SagaId = sagaId;
        }
        else if (SagaId != sagaId)
        {
            return SaleErrors.SagaMismatch;
        }

        PaymentRecorded = true;
        return Result.Updated;
    }

    public Result<Updated> Post()
    {
        // Legacy API now delegates to Confirm to preserve invariants
        return Confirm(SagaId ?? Guid.Empty);
    }

    public Result<Updated> MarkPosted()
    {
        return Confirm(SagaId ?? Guid.Empty);
    }

    public Result<Updated> Cancel()
    {
        if (Status == SaleStatus.Canceled) return SaleErrors.AlreadyCancelled;
        if (Status == SaleStatus.Confirmed) return SaleErrors.CannotCancelConfirmed;

        Status = SaleStatus.Canceled;
        return Result.Updated;
    }

    public Result<Updated> AssignExitCard(string? notes)
    {
        if (ExitCard is not null) return SaleErrors.ExitCardAlreadyAssigned;

        var patientId = Diagnosis.PatientId;
        var exitCardResult = ExitCard.Create(patientId, notes);
        if (exitCardResult.IsError) return exitCardResult.Errors;

        ExitCard = exitCardResult.Value;
        ExitCard.AssignSale(this);

        return Result.Updated;
    }
    public void MarkCreated()
    {
        AddDomainEvent(new SaleCreatedDomainEvent(
            Id,
            DiagnosisId
        ));
    }

}
