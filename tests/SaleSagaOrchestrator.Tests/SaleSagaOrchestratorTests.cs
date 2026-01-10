using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AlatrafClinic.Application.Commands;
using AlatrafClinic.Application.Sagas;
using AlatrafClinic.Application.Features.Diagnosises.Services.CreateDiagnosis;
using AlatrafClinic.Domain.Departments;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.Enums;
using AlatrafClinic.Domain.Diagnosises.InjuryReasons;
using AlatrafClinic.Domain.Diagnosises.InjurySides;
using AlatrafClinic.Domain.Diagnosises.InjuryTypes;
using AlatrafClinic.Domain.Inventory.Items;
using AlatrafClinic.Domain.Inventory.Stores;
using AlatrafClinic.Domain.Inventory.Units;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.Patients.Enums;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.People;
using AlatrafClinic.Domain.Sales;
using AlatrafClinic.Domain.Sales.Enums;
using AlatrafClinic.Domain.Services.Tickets;
using AlatrafClinic.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AlatrafClinic.Application.Tests;

public class SaleSagaOrchestratorTests
{
    private static AlatrafClinicDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AlatrafClinicDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new AlatrafClinicDbContext(options);
    }

    private static async Task<(SaleSagaOrchestrator orchestrator, AlatrafClinicDbContext db, Guid sagaId, StartSaleSagaCommand start, ItemUnit itemUnit, decimal unitPrice)> SeedAsync()
    {
        var db = CreateDbContext();
        var logger = NullLogger<SaleSagaOrchestrator>.Instance;
        var diagnosisService = new FakeDiagnosisCreationService(db);

        // Units & Item
        var unit = GeneralUnit.Create("pcs").Value;
        db.Units.Add(unit);

        var item = Item.Create("Bandage", unit).Value;
        var addUnit = item.AddOrUpdateItemUnit(unit.Id, 25m);
        if (addUnit.IsError) throw new InvalidOperationException(string.Join(';', addUnit.Errors.Select(e => e.Code)));
        db.Items.Add(item);

        // Department & Service
        var department = Department.Create("ER").Value;
        db.Departments.Add(department);
        await db.SaveChangesAsync(CancellationToken.None);
        var service = department.AddService("Visit").Value;
        db.Services.Add(service);

        // Person & Patient
        var person = Person.Create("John Doe", new DateOnly(1990, 1, 1), "771234567", null, "Addr", true).Value;
        db.People.Add(person);
        await db.SaveChangesAsync(CancellationToken.None);
        var patient = Patient.Create(person.Id, PatientType.Normal).Value;
        patient.Person = person;
        db.Patients.Add(patient);

        // Ticket
        var ticket = Ticket.Create(patient, service).Value;
        db.Tickets.Add(ticket);

        // Injury metadata
        var reason = InjuryReason.Create("fall").Value;
        var side = InjurySide.Create("left").Value;
        var type = InjuryType.Create("bruise").Value;
        db.InjuryReasons.Add(reason);
        db.InjurySides.Add(side);
        db.InjuryTypes.Add(type);

        await db.SaveChangesAsync(CancellationToken.None);

        // Store + stock
        var store = Store.Create("Main").Value;
        db.Stores.Add(store);
        await db.SaveChangesAsync(CancellationToken.None);
        var itemUnitEntity = item.ItemUnits.Single();
        var addStock = store.AddItemUnit(itemUnitEntity, 10m);
        if (addStock.IsError) throw new InvalidOperationException(string.Join(';', addStock.Errors.Select(e => e.Code)));
        db.StoreItemUnits.AddRange(store.StoreItemUnits);

        await db.SaveChangesAsync(CancellationToken.None);

        var sagaId = Guid.NewGuid();
        var start = new StartSaleSagaCommand(
            sagaId,
            ticket.Id,
            "diagnosis",
            new DateOnly(2024, 12, 31),
            new List<int> { reason.Id },
            new List<int> { side.Id },
            new List<int> { type.Id },
            new List<SaleItemInput> { new(item.Id, itemUnitEntity.Id, 2, itemUnitEntity.Price) },
            "notes");

        var orchestrator = new SaleSagaOrchestrator(db, logger, diagnosisService);
        return (orchestrator, db, sagaId, start, itemUnitEntity, itemUnitEntity.Price);
    }

    // Minimal fake diagnosis creation service for tests
    private sealed class FakeDiagnosisCreationService : IDiagnosisCreationService
    {
        private readonly AlatrafClinicDbContext _db;

        public FakeDiagnosisCreationService(AlatrafClinicDbContext db)
        {
            _db = db;
        }

        public async Task<AlatrafClinic.Domain.Common.Results.Result<Diagnosis>> CreateAsync(
            int ticketId,
            string diagnosisText,
            DateOnly injuryDate,
            List<int> injuryReasons,
            List<int> injurySides,
            List<int> injuryTypes,
            DiagnosisType diagnosisType,
            CancellationToken ct)
        {
            var existing = await _db.Diagnoses
                .Include(d => d.Ticket)
                .Include(d => d.Patient)
                .FirstOrDefaultAsync(d => d.TicketId == ticketId, ct);
            if (existing is not null)
            {
                return existing;
            }

            var reasons = await _db.InjuryReasons.Where(r => injuryReasons.Contains(r.Id)).ToListAsync(ct);
            var sides = await _db.InjurySides.Where(r => injurySides.Contains(r.Id)).ToListAsync(ct);
            var types = await _db.InjuryTypes.Where(r => injuryTypes.Contains(r.Id)).ToListAsync(ct);

            Ticket? ticket = await _db.Tickets.FindAsync(new object[] { ticketId }, ct);
#pragma warning disable CS8601 // Test-only: in-memory finder may return null, handled below
            Patient? patientCandidate;
            if (ticket is null)
            {
                patientCandidate = await _db.Patients.FirstAsync(ct);
            }
            else
            {
                patientCandidate = await _db.Patients.FindAsync(new object[] { ticket.PatientId }, ct) ?? await _db.Patients.FirstAsync(ct);
            }
#pragma warning restore CS8601

            if (patientCandidate is null)
            {
                throw new InvalidOperationException("Patient required for diagnosis");
            }

            var patient = patientCandidate;
            var patientId = patient.Id;

            var diagnosis = Diagnosis.Create(ticketId, diagnosisText, injuryDate, reasons, sides, types, patientId, diagnosisType);
            if (diagnosis.IsError) return diagnosis.Errors;

            var value = diagnosis.Value;
            if (ticket is not null)
            {
                value.Ticket = ticket;
            }

            if (patient is not null)
            {
                value.Patient = patient;
            }

            return value;
        }
    }

    [Fact]
    public async Task Start_Reserve_Confirm_Payment_HappyPath()
    {
        var (orchestrator, db, sagaId, start, itemUnit, price) = await SeedAsync();
        var ct = CancellationToken.None;

        var startResult = await orchestrator.StartAsync(start, ct);
        Assert.True(startResult.Success);
        Assert.NotNull(startResult.SaleId);

        var reserve = new ReserveInventoryCommand(sagaId, startResult.SaleId!.Value, new[] { new SaleItemReservationRequest(itemUnit.Id, 2) });
        var reserveResult = await orchestrator.ReserveInventoryAsync(reserve, ct);
        Assert.True(reserveResult.Success);

        var confirm = new ConfirmSaleCommand(sagaId, startResult.SaleId.Value);
        var confirmResult = await orchestrator.ConfirmSaleAsync(confirm, ct);
        Assert.True(confirmResult.Success);

        var payment = new CreatePaymentCommand(sagaId, startResult.SaleId.Value, price * 2);
        var paymentResult = await orchestrator.CreatePaymentAsync(payment, ct);
        Assert.True(paymentResult.Success);

        Assert.Equal(1, await db.InventoryReservations.CountAsync(ct));
        Assert.Equal(SaleStatus.Confirmed, (await db.Sales.FindAsync(new object[] { startResult.SaleId!.Value }, ct))!.Status);
        Assert.Equal(1, await db.Payments.CountAsync(ct));
    }

    [Fact]
    public async Task Idempotent_RetrySameStep_ReturnsSuccess_NoDuplicates()
    {
        var (orchestrator, db, sagaId, start, itemUnit, price) = await SeedAsync();
        var ct = CancellationToken.None;

        var startResult1 = await orchestrator.StartAsync(start, ct);
        var startResult2 = await orchestrator.StartAsync(start, ct);
        Assert.True(startResult1.Success);
        Assert.True(startResult2.Success);
        Assert.Equal(startResult1.SaleId, startResult2.SaleId);

        var reserve = new ReserveInventoryCommand(sagaId, startResult1.SaleId!.Value, new[] { new SaleItemReservationRequest(itemUnit.Id, 2) });
        var reserve1 = await orchestrator.ReserveInventoryAsync(reserve, ct);
        var reserve2 = await orchestrator.ReserveInventoryAsync(reserve, ct);
        Assert.True(reserve1.Success);
        Assert.True(reserve2.Success);

        var confirm = new ConfirmSaleCommand(sagaId, startResult1.SaleId.Value);
        var confirm1 = await orchestrator.ConfirmSaleAsync(confirm, ct);
        var confirm2 = await orchestrator.ConfirmSaleAsync(confirm, ct);
        Assert.True(confirm1.Success);
        Assert.True(confirm2.Success);

        var payment = new CreatePaymentCommand(sagaId, startResult1.SaleId.Value, price * 2);
        var payment1 = await orchestrator.CreatePaymentAsync(payment, ct);
        var payment2 = await orchestrator.CreatePaymentAsync(payment, ct);
        Assert.True(payment1.Success);
        Assert.True(payment2.Success);

        Assert.Equal(1, await db.InventoryReservations.CountAsync(ct));
        Assert.Equal(1, await db.Payments.CountAsync(ct));
    }

    [Fact]
    public async Task Confirm_Before_Reserve_ShouldFail()
    {
        var (orchestrator, _, sagaId, start, itemUnit, _) = await SeedAsync();
        var ct = CancellationToken.None;
        var startResult = await orchestrator.StartAsync(start, ct);
        var confirm = new ConfirmSaleCommand(sagaId, startResult.SaleId!.Value);
        var confirmResult = await orchestrator.ConfirmSaleAsync(confirm, ct);
        Assert.False(confirmResult.Success);
    }

    [Fact]
    public async Task Payment_Before_Confirm_ShouldFail()
    {
        var (orchestrator, _, sagaId, start, itemUnit, price) = await SeedAsync();
        var ct = CancellationToken.None;
        var startResult = await orchestrator.StartAsync(start, ct);
        var reserve = new ReserveInventoryCommand(sagaId, startResult.SaleId!.Value, new[] { new SaleItemReservationRequest(itemUnit.Id, 2) });
        await orchestrator.ReserveInventoryAsync(reserve, ct);
        var payment = new CreatePaymentCommand(sagaId, startResult.SaleId.Value, price * 2);
        var paymentResult = await orchestrator.CreatePaymentAsync(payment, ct);
        Assert.False(paymentResult.Success);
    }

    [Fact]
    public async Task Reserve_With_Insufficient_Stock_Fails()
    {
        var (orchestrator, db, sagaId, start, itemUnit, _) = await SeedAsync();
        var ct = CancellationToken.None;
        var startResult = await orchestrator.StartAsync(start, ct);

        // Simulate depletion between start and reserve
        var storeUnit = await db.StoreItemUnits.FirstAsync(ct);
        var reduce = storeUnit.Decrease(storeUnit.Quantity - 1);
        if (reduce.IsError) throw new InvalidOperationException(string.Join(';', reduce.Errors.Select(e => e.Code)));
        await db.SaveChangesAsync(ct);

        var reserve = new ReserveInventoryCommand(sagaId, startResult.SaleId!.Value, new[] { new SaleItemReservationRequest(itemUnit.Id, 999) });
        var reserveResult = await orchestrator.ReserveInventoryAsync(reserve, ct);
        Assert.False(reserveResult.Success);
    }

    [Fact]
    public async Task Start_With_Price_Mismatch_Fails()
    {
        var (orchestrator, _, sagaId, start, itemUnit, _) = await SeedAsync();
        var badStart = start with
        {
            Items = new List<SaleItemInput> { new(itemUnit.ItemId, itemUnit.Id, 1, itemUnit.Price + 1) }
        };
        var result = await orchestrator.StartAsync(badStart, CancellationToken.None);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task CrossSaga_On_Reserve_ShouldFail()
    {
        var (orchestrator, _, sagaId, start, itemUnit, _) = await SeedAsync();
        var ct = CancellationToken.None;
        var startResult = await orchestrator.StartAsync(start, ct);
        var reserve = new ReserveInventoryCommand(sagaId, startResult.SaleId!.Value, new[] { new SaleItemReservationRequest(itemUnit.Id, 2) });
        await orchestrator.ReserveInventoryAsync(reserve, ct);

        var otherSaga = Guid.NewGuid();
        var reserveOther = new ReserveInventoryCommand(otherSaga, startResult.SaleId.Value, new[] { new SaleItemReservationRequest(itemUnit.Id, 2) });
        var reserveOtherResult = await orchestrator.ReserveInventoryAsync(reserveOther, ct);
        Assert.False(reserveOtherResult.Success);
    }

    [Fact]
    public async Task Concurrent_Reserve_OnlyOne_Succeeds()
    {
        var (orchestrator, db, sagaId, start, itemUnit, _) = await SeedAsync();
        var ct = CancellationToken.None;
        var startResult = await orchestrator.StartAsync(start, ct);

        var reserveCmd = new ReserveInventoryCommand(sagaId, startResult.SaleId!.Value, new[] { new SaleItemReservationRequest(itemUnit.Id, 2) });
        var tasks = Enumerable.Range(0, 3)
            .Select(_ => orchestrator.ReserveInventoryAsync(reserveCmd, ct));

        var results = await Task.WhenAll(tasks);
        Assert.Contains(results, r => r.Success);
        Assert.Equal(1, await db.InventoryReservations.CountAsync(ct));
    }

    [Fact]
    public async Task Concurrent_Payment_OnlyOne_Created()
    {
        var (orchestrator, db, sagaId, start, itemUnit, price) = await SeedAsync();
        var ct = CancellationToken.None;
        var startResult = await orchestrator.StartAsync(start, ct);
        var reserve = new ReserveInventoryCommand(sagaId, startResult.SaleId!.Value, new[] { new SaleItemReservationRequest(itemUnit.Id, 2) });
        await orchestrator.ReserveInventoryAsync(reserve, ct);
        await orchestrator.ConfirmSaleAsync(new ConfirmSaleCommand(sagaId, startResult.SaleId!.Value), ct);

        var paymentCmd = new CreatePaymentCommand(sagaId, startResult.SaleId.Value, price * 2);
        var tasks = Enumerable.Range(0, 3).Select(_ => orchestrator.CreatePaymentAsync(paymentCmd, ct));
        var results = await Task.WhenAll(tasks);
        Assert.Contains(results, r => r.Success);
        Assert.Equal(1, await db.Payments.CountAsync(ct));
    }

    [Fact]
    public async Task Payment_With_Incorrect_Total_Still_Uses_Request_Total()
    {
        var (orchestrator, db, sagaId, start, itemUnit, price) = await SeedAsync();
        var ct = CancellationToken.None;
        var startResult = await orchestrator.StartAsync(start, ct);
        await orchestrator.ReserveInventoryAsync(new ReserveInventoryCommand(sagaId, startResult.SaleId!.Value, new[] { new SaleItemReservationRequest(itemUnit.Id, 2) }), ct);
        await orchestrator.ConfirmSaleAsync(new ConfirmSaleCommand(sagaId, startResult.SaleId!.Value), ct);

        var payment = new CreatePaymentCommand(sagaId, startResult.SaleId.Value, price * 3); // mismatch vs sale total (2 * price)
        var paymentResult = await orchestrator.CreatePaymentAsync(payment, ct);
        Assert.True(paymentResult.Success);
        var storedPayment = await db.Payments.SingleAsync(ct);
        Assert.Equal(price * 3, storedPayment.TotalAmount);
    }
}
