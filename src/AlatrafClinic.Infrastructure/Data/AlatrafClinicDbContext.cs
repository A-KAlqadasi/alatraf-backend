using System;
using System.Linq;
using System.Text.Json;
using AlatrafClinic.Domain.Common;


using AlatrafClinic.Application.Common.Interfaces;
using MediatR;
using AlatrafClinic.Domain.Departments;
using AlatrafClinic.Domain.Departments.DoctorSectionRooms;
using AlatrafClinic.Domain.Departments.Sections;
using AlatrafClinic.Domain.Departments.Sections.Rooms;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.DiagnosisIndustrialParts;
using AlatrafClinic.Domain.Diagnosises.DiagnosisPrograms;
using AlatrafClinic.Domain.Diagnosises.InjuryReasons;
using AlatrafClinic.Domain.Diagnosises.InjurySides;
using AlatrafClinic.Domain.Diagnosises.InjuryTypes;
using AlatrafClinic.Domain.DisabledCards;
using AlatrafClinic.Domain.Identity;
using AlatrafClinic.Domain.Inventory.ExchangeOrders;
using AlatrafClinic.Domain.Inventory.Items;
using AlatrafClinic.Domain.Inventory.Purchases;
using AlatrafClinic.Domain.Inventory.Stores;
using AlatrafClinic.Domain.Inventory.Suppliers;
using AlatrafClinic.Domain.Inventory.Units;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.Payments.DisabledPayments;
using AlatrafClinic.Domain.Payments.PatientPayments;
using AlatrafClinic.Domain.Payments.WoundedPayments;
using AlatrafClinic.Domain.People;
using AlatrafClinic.Domain.People.Doctors;
using AlatrafClinic.Domain.RepairCards;
using AlatrafClinic.Domain.RepairCards.IndustrialParts;
using AlatrafClinic.Domain.RepairCards.Orders;
using AlatrafClinic.Domain.Reports;
using AlatrafClinic.Domain.Sales;
using AlatrafClinic.Domain.Sales.SalesItems;
using AlatrafClinic.Domain.Services;
using AlatrafClinic.Domain.Services.Appointments;
using AlatrafClinic.Domain.Services.Appointments.Holidays;
using AlatrafClinic.Domain.Services.Tickets;
using AlatrafClinic.Domain.Settings;
using AlatrafClinic.Domain.TherapyCards;
using AlatrafClinic.Domain.TherapyCards.MedicalPrograms;
using AlatrafClinic.Domain.TherapyCards.Sessions;
using AlatrafClinic.Domain.TherapyCards.TherapyCardTypePrices;
using AlatrafClinic.Domain.WoundedCards;
using AlatrafClinic.Infrastructure.Identity;


using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AlatrafClinic.Infrastructure.Data.Idempotency;
using AlatrafClinic.Infrastructure.Data.Outbox;
using AlatrafClinic.Domain.Sagas;


namespace AlatrafClinic.Infrastructure.Data;

public class AlatrafClinicDbContext
    : IdentityDbContext<AppUser, IdentityRole, string>, IAppDbContext
{
    // Collected domain events are stored here during SaveChanges
    private readonly List<INotification> _collectedDomainEvents = new();

    public AlatrafClinicDbContext(DbContextOptions<AlatrafClinicDbContext> options)
        : base(options)
    {
    }


    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<ApplicationPermission> Permissions => Set<ApplicationPermission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<UserPermissionOverride> UserPermissionOverrides => Set<UserPermissionOverride>();

    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Holiday> Holidays => Set<Holiday>();

    public DbSet<Diagnosis> Diagnoses => Set<Diagnosis>();
    public DbSet<InjuryType> InjuryTypes => Set<InjuryType>();
    public DbSet<InjurySide> InjurySides => Set<InjurySide>();
    public DbSet<InjuryReason> InjuryReasons => Set<InjuryReason>();
    public DbSet<DiagnosisIndustrialPart> DiagnosisIndustrialParts => Set<DiagnosisIndustrialPart>();
    public DbSet<DiagnosisProgram> DiagnosisPrograms => Set<DiagnosisProgram>();

    public DbSet<TherapyCard> TherapyCards => Set<TherapyCard>();
    public DbSet<MedicalProgram> MedicalPrograms => Set<MedicalProgram>();
    public DbSet<TherapyCardTypePrice> TherapyCardTypePrices => Set<TherapyCardTypePrice>();

    public DbSet<RepairCard> RepairCards => Set<RepairCard>();
    public DbSet<IndustrialPart> IndustrialParts => Set<IndustrialPart>();
    public DbSet<IndustrialPartUnit> IndustrialPartUnits => Set<IndustrialPartUnit>();

    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<WoundedPayment> WoundedPayments => Set<WoundedPayment>();
    public DbSet<DisabledPayment> DisabledPayments => Set<DisabledPayment>();
    public DbSet<PatientPayment> PatientPayments => Set<PatientPayment>();

    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<SessionProgram> SessionPrograms => Set<SessionProgram>();

    public DbSet<Service> Services => Set<Service>();
    public DbSet<Ticket> Tickets => Set<Ticket>();

    public DbSet<Person> People => Set<Person>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Doctor> Doctors => Set<Doctor>();

    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Section> Sections => Set<Section>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<DoctorSectionRoom> DoctorSectionRooms => Set<DoctorSectionRoom>();

    public DbSet<DisabledCard> DisabledCards => Set<DisabledCard>();
    public DbSet<WoundedCard> WoundedCards => Set<WoundedCard>();

    public DbSet<AppSetting> AppSettings => Set<AppSetting>();


    // Inventory entities
    public DbSet<Item> Items => Set<Item>();
    public DbSet<ItemUnit> ItemUnits => Set<ItemUnit>();
    public DbSet<GeneralUnit> Units => Set<GeneralUnit>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<StoreItemUnit> StoreItemUnits => Set<StoreItemUnit>();
    public DbSet<ExchangeOrder> ExchangeOrders => Set<ExchangeOrder>();
    public DbSet<PurchaseInvoice> PurchaseInvoices => Set<PurchaseInvoice>();

    public DbSet<PurchaseItem> PurchaseItems => Set<PurchaseItem>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<InventoryReservation> InventoryReservations => Set<InventoryReservation>();
    public DbSet<IdempotencyKey> IdempotencyKeys => Set<IdempotencyKey>();
    public DbSet<ProcessedMessage> ProcessedMessages => Set<ProcessedMessage>();
    public DbSet<ReportDomain> ReportDomains => Set<ReportDomain>();
    public DbSet<ReportField> ReportFields => Set<ReportField>();
    public DbSet<ReportJoin> ReportJoins => Set<ReportJoin>();

    public DbSet<SagaState> SagaStates => Set<SagaState>();
    public DbSet<SagaStepRecord> SagaStepRecords => Set<SagaStepRecord>();
    public DbSet<SagaCompensationLog> SagaCompensationLogs => Set<SagaCompensationLog>();
    public DbSet<CompensationNotification> CompensationNotifications => Set<CompensationNotification>();
    public DbSet<ManualInterventionRequired> ManualInterventions => Set<ManualInterventionRequired>();

    public DbSet<ReportDomain> Domains => Set<ReportDomain>();
    public DbSet<ReportField> Fields => Set<ReportField>();
    public DbSet<ReportJoin> Joins => Set<ReportJoin>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AlatrafClinicDbContext).Assembly);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Collect domain events from tracked entities into the in-memory buffer
        var domainEvents = ChangeTracker
            .Entries<Entity<int>>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .SelectMany(e => e.DomainEvents)
            .Cast<INotification>()
            .ToList();

        if (domainEvents.Any())
        {
            _collectedDomainEvents.AddRange(domainEvents);
        }

        // Clear domain events from entities so they are not published twice
        foreach (var entity in ChangeTracker.Entries<Entity<int>>())
        {
            entity.Entity.ClearDomainEvents();
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    // Called by the pipeline behavior after a successful handler execution / commit
    public IReadOnlyCollection<INotification> GetDomainEvents()
    {
        var events = _collectedDomainEvents.ToList();
        _collectedDomainEvents.Clear();
        return events;
    }

}