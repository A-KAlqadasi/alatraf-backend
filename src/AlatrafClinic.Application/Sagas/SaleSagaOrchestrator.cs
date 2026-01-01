using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AlatrafClinic.Application.Commands;
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Diagnosises.Services.CreateDiagnosis;
using AlatrafClinic.Domain.Diagnosises.Enums;
using AlatrafClinic.Domain.Inventory.Reservations;
using AlatrafClinic.Domain.Inventory.Stores;
using AlatrafClinic.Domain.Sales;
using AlatrafClinic.Domain.Sales.Enums;
using AlatrafClinic.Domain.Sales.SalesItems;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Sagas;

// Centralized orchestrator; deterministic, replay-safe, application-layer only.
public sealed class SaleSagaOrchestrator
{
    private readonly IAppDbContext _db;
    private readonly ILogger<SaleSagaOrchestrator> _logger;
    private readonly IDiagnosisCreationService _diagnosisService;

    public SaleSagaOrchestrator(IAppDbContext db, ILogger<SaleSagaOrchestrator> logger, IDiagnosisCreationService diagnosisService)
    {
        _db = db;
        _logger = logger;
        _diagnosisService = diagnosisService;
    }

    public async Task<SaleSagaResult> StartAsync(StartSaleSagaCommand command, CancellationToken ct)
    {
        var sagaId = command.SagaId == Guid.Empty ? Guid.NewGuid() : command.SagaId;

        // Step 1: Validate stock upfront (fail fast; no partial reservation)
        var stockOk = await ValidateStockAsync(command.Items, ct);
        if (!stockOk)
        {
            return SaleSagaResult.Fail("Insufficient stock for one or more items.");
        }

        // Create Diagnosis for the sale if not exists
        // (handled in CreateSaleDraftAsync)
        var diagnosisResult = await _diagnosisService.CreateAsync(
            command.TicketId,
            command.DiagnosisText,
            command.InjuryDate,
            command.InjuryReasons,
            command.InjurySides,
            command.InjuryTypes,
            DiagnosisType.Sales,
            ct);
        if (diagnosisResult.IsError)
        {
            _logger.LogError(
                "Failed to create Diagnosis for Ticket {ticketId}: {Errors}",
                command.TicketId,
                string.Join(", ", diagnosisResult.Errors));

            return SaleSagaResult.Fail(diagnosisResult.Errors.Select(e => e.Description).ToArray());
        }
        var diagnosis = diagnosisResult.Value;
        _db.Diagnoses.Attach(diagnosis);
        

        await _db.SaveChangesAsync(ct);
        //logging the created diagnosis id
        _logger.LogInformation("Created Diagnosis {DiagnosisId} for Ticket {TicketId}", diagnosisResult.Value.Id, command.TicketId);


        // Step 2: Create sale draft (idempotent on sagaId + ticket)
        var draftResult = await CreateSaleDraftAsync(sagaId, command, ct);
        if (!draftResult.Success)
        {
            return draftResult;
        }

        return draftResult;
    }

    public async Task<SaleSagaResult> ReserveInventoryAsync(ReserveInventoryCommand command, CancellationToken ct)
    {
        // Idempotent: if reservations exist for sagaId+saleId, return success
        // Load sale with items
        var sale = await _db.Sales
            .Include(s => s.SaleItems)
            .ThenInclude(si => si.ItemUnit)
            .FirstOrDefaultAsync(s => s.Id == command.SaleId, ct);

        if (sale is null)
        {
            return SaleSagaResult.Fail($"Sale {command.SaleId} not found.");
        }

        if (sale.SagaId is null)
        {
            var attach = sale.AttachSaga(command.SagaId);
            if (attach.IsError)
            {
                return SaleSagaResult.Fail(attach.Errors.Select(e => e.Description).ToArray());
            }
            await _db.SaveChangesAsync(ct);
        }
        else if (sale.SagaId != command.SagaId)
        {
            return SaleSagaResult.Fail("Saga mismatch for reservation step.");
        }

        if (sale.InventoryReservationCompleted)
        {
            _logger.LogInformation("Saga {SagaId}: inventory already reserved for sale {SaleId}", command.SagaId, command.SaleId);
            if (sale.SagaId is null)
            {
                var attachReserved = sale.AttachSaga(command.SagaId);
                if (attachReserved.IsError)
                {
                    return SaleSagaResult.Fail(attachReserved.Errors.Select(e => e.Description).ToArray());
                }
                await _db.SaveChangesAsync(ct);
            }
            return SaleSagaResult.Ok(command.SaleId, sale.Total);
        }

        var existing = await _db.InventoryReservations
            .AnyAsync(r => r.SagaId == command.SagaId && r.SaleId == command.SaleId, ct);

        if (existing)
        {
            _logger.LogInformation("Saga {SagaId}: inventory already reserved for sale {SaleId}", command.SagaId, command.SaleId);
            var attach = sale.MarkInventoryReserved(command.SagaId);
            if (attach.IsError)
            {
                return SaleSagaResult.Fail(attach.Errors.Select(e => e.Description).ToArray());
            }

            await _db.SaveChangesAsync(ct);
            return SaleSagaResult.Ok(command.SaleId, sale.Total);
        }

        // Atomic reservation: validate all stock upfront before any mutation
        var saleItemUnits = sale.SaleItems.Select(si => si.ItemUnitId).ToList();
        var storeUnits = await _db.StoreItemUnits
            .Where(x => saleItemUnits.Contains(x.ItemUnitId))
            .ToListAsync(ct);

        foreach (var saleItem in sale.SaleItems)
        {
            var stock = storeUnits.FirstOrDefault(su => su.ItemUnitId == saleItem.ItemUnitId);
            if (stock is null || stock.Quantity < saleItem.Quantity)
            {
                return SaleSagaResult.Fail($"Insufficient stock for ItemUnit {saleItem.ItemUnitId}");
            }
        }

        if (_db is not DbContext efDb)
        {
            return SaleSagaResult.Fail("DbContext not available for transaction.");
        }

        using var tx = await efDb.Database.BeginTransactionAsync(ct);
        foreach (var saleItem in sale.SaleItems)
        {
            var storeItemUnit = storeUnits.First(su => su.ItemUnitId == saleItem.ItemUnitId);

            var decrease = storeItemUnit.Decrease(saleItem.Quantity);
            if (decrease.IsError)
            {
                await tx.RollbackAsync(ct);
                return SaleSagaResult.Fail($"Insufficient stock for ItemUnit {saleItem.ItemUnitId}");
            }

            saleItem.AssignStoreItemUnit(storeItemUnit);
            var reservation = InventoryReservation.Create(command.SagaId, sale.Id, storeItemUnit.Id, saleItem.Quantity);
            await _db.InventoryReservations.AddAsync(reservation, ct);
        }

        var markReserved = sale.MarkInventoryReserved(command.SagaId);
        if (markReserved.IsError)
        {
            await tx.RollbackAsync(ct);
            return SaleSagaResult.Fail(markReserved.Errors.Select(e => e.Description).ToArray());
        }

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return SaleSagaResult.Ok(sale.Id, sale.Total);
    }

    public async Task<SaleSagaResult> ConfirmSaleAsync(ConfirmSaleCommand command, CancellationToken ct)
    {
        var sale = await _db.Sales.FirstOrDefaultAsync(s => s.Id == command.SaleId, ct);
        if (sale is null)
        {
            return SaleSagaResult.Fail($"Sale {command.SaleId} not found.");
        }

        if (sale.SagaId is null)
        {
            var attach = sale.AttachSaga(command.SagaId);
            if (attach.IsError)
            {
                return SaleSagaResult.Fail(attach.Errors.Select(e => e.Description).ToArray());
            }
            await _db.SaveChangesAsync(ct);
        }
        else if (sale.SagaId != command.SagaId)
        {
            return SaleSagaResult.Fail("Saga mismatch for confirm step.");
        }

        if (!sale.InventoryReservationCompleted)
        {
            return SaleSagaResult.Fail("Inventory not reserved; cannot confirm sale.");
        }
        if (sale.Status == SaleStatus.Confirmed)
        {
            return SaleSagaResult.Ok(sale.Id, sale.Total);
        }

        if (_db is not DbContext efDb)
        {
            return SaleSagaResult.Fail("DbContext not available for transaction.");
        }

        await using var tx = await efDb.Database.BeginTransactionAsync(ct);

        var result = sale.Confirm(command.SagaId);
        if (result.IsError)
        {
            await tx.RollbackAsync(ct);
            return SaleSagaResult.Fail(result.Errors.Select(e => e.Description).ToArray());
        }

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return SaleSagaResult.Ok(sale.Id, sale.Total);
    }

    public async Task<SaleSagaResult> CreatePaymentAsync(CreatePaymentCommand command, CancellationToken ct)
    {
        var sale = await _db.Sales
            .Include(s => s.Diagnosis)
            .ThenInclude(d => d.Payments)
            .FirstOrDefaultAsync(s => s.Id == command.SaleId, ct);

        if (sale is null)
        {
            return SaleSagaResult.Fail($"Sale {command.SaleId} not found.");
        }

        // Guard ordering/idempotency: payment only after reservation + confirmation and within the same saga
        if (sale.SagaId is not null && sale.SagaId != command.SagaId)
        {
            return SaleSagaResult.Fail("Saga mismatch for payment step.");
        }
        if (!sale.InventoryReservationCompleted)
        {
            return SaleSagaResult.Fail("Inventory not reserved; payment is blocked.");
        }
        if (sale.Status != SaleStatus.Confirmed)
        {
            return SaleSagaResult.Fail("Sale must be confirmed before payment.");
        }

        if (_db is not DbContext efDb)
        {
            return SaleSagaResult.Fail("DbContext not available for transaction.");
        }

        await using var tx = await efDb.Database.BeginTransactionAsync(ct);

        // Idempotent: existing payment with sagaId
        var existing = sale.Diagnosis.Payments.FirstOrDefault(p => p.SagaId == command.SagaId && p.PaymentReference == Domain.Payments.PaymentReference.Sales);
        if (existing is not null)
        {
            await tx.CommitAsync(ct);
            return SaleSagaResult.Ok(sale.Id, sale.Total);
        }

        // Idempotent replay: if already marked, ensure same saga
        if (sale.PaymentRecorded && sale.SagaId == command.SagaId)
        {
            await tx.CommitAsync(ct);
            return SaleSagaResult.Ok(sale.Id, sale.Total);
        }
        if (sale.PaymentRecorded && sale.SagaId != command.SagaId)
        {
            await tx.RollbackAsync(ct);
            return SaleSagaResult.Fail("Payment already recorded by a different saga.");
        }

        var paymentResult = Domain.Payments.Payment.Create(command.SagaId, sale.Diagnosis.TicketId, sale.DiagnosisId, command.Total, Domain.Payments.PaymentReference.Sales);
        if (paymentResult.IsError)
        {
            await tx.RollbackAsync(ct);
            return SaleSagaResult.Fail(paymentResult.Errors.Select(e => e.Description).ToArray());
        }

        var payment = paymentResult.Value;
        sale.Diagnosis.AssignPayment(payment);

        await _db.Payments.AddAsync(payment, ct);

        var markPayment = sale.MarkPaymentCreated(command.SagaId);
        if (markPayment.IsError)
        {
            await tx.RollbackAsync(ct);
            return SaleSagaResult.Fail(markPayment.Errors.Select(e => e.Description).ToArray());
        }

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return SaleSagaResult.Ok(sale.Id, sale.Total);
    }

    private async Task<bool> ValidateStockAsync(IReadOnlyCollection<SaleItemInput> items, CancellationToken ct)
    {
        if (!items.Any()) return false;

        var itemUnitIds = items.Select(i => i.UnitId).ToList();
        var stocks = await _db.StoreItemUnits
            .Where(siu => itemUnitIds.Contains(siu.ItemUnitId))
            .ToListAsync(ct);

        foreach (var item in items)
        {
            var stock = stocks.FirstOrDefault(s => s.ItemUnitId == item.UnitId);
            if (stock is null) return false;
            if (stock.Quantity < item.Quantity) return false;
        }

        return true;
    }

    private async Task<SaleSagaResult> CreateSaleDraftAsync(Guid sagaId, StartSaleSagaCommand command, CancellationToken ct)
    {
        // Idempotent: existing sale for this ticket and saga
        var existingSale = await _db.Sales
            .Include(s => s.SaleItems)
            .FirstOrDefaultAsync(s => s.Diagnosis.TicketId == command.TicketId, ct);

        if (existingSale is not null)
        {
            if (existingSale.SagaId is null)
            {
                // Backward compatibility: attach saga once
                var attach = existingSale.AttachSaga(sagaId);
                if (attach.IsError)
                {
                    return SaleSagaResult.Fail(attach.Errors.Select(e => e.Description).ToArray());
                }

                await _db.SaveChangesAsync(ct);
                return SaleSagaResult.Ok(existingSale.Id, existingSale.Total);
            }

            if (existingSale.SagaId != sagaId)
            {
                return SaleSagaResult.Fail("Saga mismatch for existing sale draft.");
            }

            return SaleSagaResult.Ok(existingSale.Id, existingSale.Total);
        }

        var diagnosis = await _db.Diagnoses
            .Include(d => d.InjuryReasons)
            .Include(d => d.InjurySides)
            .Include(d => d.InjuryTypes)
            .FirstOrDefaultAsync(d => d.TicketId == command.TicketId, ct);

        if (diagnosis is null)
        {
            return SaleSagaResult.Fail($"Diagnosis for Ticket {command.TicketId} not found.");
        }

        var newItems = new System.Collections.Generic.List<(AlatrafClinic.Domain.Inventory.Items.ItemUnit itemUnit, decimal quantity)>();
        foreach (var item in command.Items)
        {
            var itemUnit = await _db.ItemUnits.FirstOrDefaultAsync(iu => iu.ItemId == item.ItemId && iu.Id == item.UnitId, ct);
            if (itemUnit is null)
            {
                return SaleSagaResult.Fail($"ItemUnit {item.UnitId} not found.");
            }
            if (itemUnit.Price != item.UnitPrice)
            {
                return SaleSagaResult.Fail($"ItemUnit {item.UnitId} price mismatch.");
            }
            newItems.Add((itemUnit, item.Quantity));
        }

        var saleResult = Sale.Create(diagnosis.Id, command.Notes);
        if (saleResult.IsError)
        {
            return SaleSagaResult.Fail(saleResult.Errors.Select(e => e.Description).ToArray());
        }

        var sale = saleResult.Value;
        var sagaAttach = sale.AttachSaga(sagaId);
        if (sagaAttach.IsError)
        {
            return SaleSagaResult.Fail(sagaAttach.Errors.Select(e => e.Description).ToArray());
        }
        var upsertResult = sale.UpsertItems(newItems);
        if (upsertResult.IsError)
        {
            return SaleSagaResult.Fail(upsertResult.Errors.Select(e => e.Description).ToArray());
        }

        diagnosis.AssignToSale(sale);

        await _db.Sales.AddAsync(sale, ct);
        await _db.SaveChangesAsync(ct);

        return SaleSagaResult.Ok(sale.Id, sale.Total);
    }
}
