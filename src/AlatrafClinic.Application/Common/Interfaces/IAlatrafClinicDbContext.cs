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


using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Common.Interfaces;

public interface IAlatrafClinicDbContext
{
    public DbSet<Appointment> Appointments { get; } 
    public DbSet<Holiday> Holidays { get; }
    
    public DbSet<Diagnosis> Diagnoses { get; }
    public DbSet<InjuryType> InjuryTypes { get; }
    public DbSet<InjurySide> InjurySides { get; }
    public DbSet<InjuryReason> InjuryReasons { get; }
    public DbSet<DiagnosisIndustrialPart> DiagnosisIndustrialParts { get; }
    public DbSet<DiagnosisProgram> DiagnosisPrograms { get; }

    public DbSet<TherapyCard> TherapyCards { get; }
    public DbSet<MedicalProgram> MedicalPrograms { get; }
    public DbSet<TherapyCardTypePrice> TherapyCardTypePrices { get; }

    public DbSet<RepairCard> RepairCards { get; }
    public DbSet<IndustrialPart> IndustrialParts { get; }
    public DbSet<IndustrialPartUnit> IndustrialPartUnits { get; }
   
    public DbSet<Sale> Sales { get; }
    public DbSet<SaleItem> SaleItems { get; }

    public DbSet<Order> Orders { get; }
    public DbSet<OrderItem> OrderItems { get; }

    public DbSet<Payment> Payments { get; }
    public DbSet<WoundedPayment> WoundedPayments { get; }
    public DbSet<DisabledPayment> DisabledPayments { get; }
    public DbSet<PatientPayment> PatientPayments { get; }

    public DbSet<Session> Sessions { get; }
    public DbSet<SessionProgram> SessionPrograms { get; }

    public DbSet<Service> Services { get; }
    public DbSet<Ticket> Tickets { get; }

    public DbSet<Person> People { get; }
    public DbSet<Patient> Patients { get; }
    public DbSet<Doctor> Doctors { get; }
   
    public DbSet<Department> Departments { get; }
    public DbSet<Section> Sections { get; }
    public DbSet<Room> Rooms { get; }
    public DbSet<DoctorSectionRoom> DoctorSectionRooms { get; }

    public DbSet<DisabledCard> DisabledCards { get; }
    public DbSet<WoundedCard> WoundedCards { get; }

    public DbSet<AppSetting> AppSettings { get; }
}