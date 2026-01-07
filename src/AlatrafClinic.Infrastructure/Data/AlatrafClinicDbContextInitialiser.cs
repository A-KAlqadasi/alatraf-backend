using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Departments;
using AlatrafClinic.Domain.Diagnosises.InjuryReasons;
using AlatrafClinic.Domain.Diagnosises.InjurySides;
using AlatrafClinic.Domain.Diagnosises.InjuryTypes;
using AlatrafClinic.Domain.Inventory.Units;
using AlatrafClinic.Domain.RepairCards.IndustrialParts;
using AlatrafClinic.Domain.Reports;
using AlatrafClinic.Domain.Services;
using AlatrafClinic.Domain.Settings;
using AlatrafClinic.Domain.TherapyCards.Enums;
using AlatrafClinic.Domain.TherapyCards.MedicalPrograms;
using AlatrafClinic.Domain.TherapyCards.TherapyCardTypePrices;
using AlatrafClinic.Infrastructure.Identity;


using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Infrastructure.Data;

public sealed class AlatrafClinicDbContextInitialiser
{
    private readonly ILogger<AlatrafClinicDbContextInitialiser> _logger;
    private readonly AlatrafClinicDbContext _context;
    
    public AlatrafClinicDbContextInitialiser(
        ILogger<AlatrafClinicDbContextInitialiser> logger,
        AlatrafClinicDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task InitialiseAsync(CancellationToken ct = default)
    {
        try
        {   
            await _context.Database.MigrateAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }
    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }
    private async Task TrySeedAsync()
    {
        int? id1 = null;
        int? id2  = null;
        if(!await _context.Departments.AnyAsync())
        {
            var dep1 = Department.Create(1, "العلاج الطبيعي").Value;
            var dep2 = Department.Create(2, "الادارة الفنية").Value;

            _context.Departments.Add(dep1);
            _context.Departments.Add(dep2);

            await _context.SaveChangesAsync();
            id1 = dep1.Id;
            id2 = dep2.Id;

        }

        // Seed, if necessary
        if (!await _context.Services.AnyAsync())
        {
            _context.Services.AddRange(
                Service.Create(1, "استشارة", null).Value,
                Service.Create(2, "علاج طبيعي", id1).Value,
                Service.Create(3, "اطراف صناعية", id2).Value,
                Service.Create(4, "مبيعات", id2).Value,
                Service.Create(5, "إصلاحات", id2).Value,
                Service.Create(6, "عظام", id1).Value,
                Service.Create(7, "أعصاب", id1).Value,
                Service.Create(8, "تجديد كروت علاج", id1).Value,
                Service.Create(9, "إصدار بدل فاقد لكرت علاج", id1, price: 500).Value
            );

        }
        if (!await _context.InjuryTypes.AnyAsync())
        {
            _context.InjuryTypes.AddRange(
                InjuryType.Create("كسر يد").Value,
                InjuryType.Create("حرق").Value,
                InjuryType.Create("كسر قدم").Value
            );
        }
        if (!await _context.InjuryReasons.AnyAsync())
        {
            _context.InjuryReasons.AddRange(
                InjuryReason.Create("حادث").Value,
                InjuryReason.Create("إجهاد").Value,
                InjuryReason.Create("مرض").Value
            );
        }
        if (!await _context.InjurySides.AnyAsync())
        {
            _context.InjurySides.AddRange(
                InjurySide.Create("اليد المينى").Value,
                InjurySide.Create("اليد اليسرى").Value,
                InjurySide.Create("الرجل اليمنى").Value,
                InjurySide.Create("الرجل اليسرى").Value
            );
        }
        if (!await _context.TherapyCardTypePrices.AnyAsync())
        {
            _context.TherapyCardTypePrices.AddRange(
                TherapyCardTypePrice.Create(TherapyCardType.General, 200m).Value,
                TherapyCardTypePrice.Create(TherapyCardType.Special, 2000m).Value,
                TherapyCardTypePrice.Create(TherapyCardType.NerveKids, 400m).Value
            );
        }

        if (!await _context.Units.AnyAsync())
        {
            _context.Units.AddRange(
                GeneralUnit.Create("قطعة").Value,
                GeneralUnit.Create("زوج").Value,
                GeneralUnit.Create("طرف علوي يمين").Value,
                GeneralUnit.Create("طرف علوي يسار").Value,
                GeneralUnit.Create("طرف سفلي يمين").Value,
                GeneralUnit.Create("طرف سفلي يسار").Value,
                GeneralUnit.Create("الطرفين العلويين").Value,
                GeneralUnit.Create("الطرفين السفليين").Value
            );
        }
        
        if (!await _context.MedicalPrograms.AnyAsync())
        {
            _context.MedicalPrograms.AddRange(
                MedicalProgram.Create("تمارين").Value,
                MedicalProgram.Create("مساج").Value,
                MedicalProgram.Create("حرارة").Value
            );
        }

        if (!await _context.IndustrialParts.AnyAsync())
        {
            _context.IndustrialParts.AddRange(
                IndustrialPart.Create("طرف صناعي").Value,
                IndustrialPart.Create("رجل صناعي").Value,
                IndustrialPart.Create("دعامة ركبة").Value
            );
        }
        if (!await _context.AppSettings.AnyAsync())
        {
            _context.AppSettings.AddRange(
                AppSetting.Create(AlatrafClinicConstants.AllowedDaysKey, "Saturday, Tuesday").Value,
                AppSetting.Create(AlatrafClinicConstants.AppointmentDailyCapacityKey, AlatrafClinicConstants.DefaultAppointmentDailyCapacity.ToString()).Value,
                AppSetting.Create(AlatrafClinicConstants.WoundedReportMinTotalKey, "30000").Value
            );
        }

        if (!await _context.Permissions.AnyAsync())
        {
            _context.Permissions.AddRange(
                // Person
                new ApplicationPermission { Name = Permission.Person.Create },
                new ApplicationPermission { Name = Permission.Person.Read },
                new ApplicationPermission { Name = Permission.Person.Update },
                new ApplicationPermission { Name = Permission.Person.Delete },

                // Service
                new ApplicationPermission { Name = Permission.Service.Create },
                new ApplicationPermission { Name = Permission.Service.Read },
                new ApplicationPermission { Name = Permission.Service.Update },
                new ApplicationPermission { Name = Permission.Service.Delete },

                // Ticket
                new ApplicationPermission { Name = Permission.Ticket.Create },
                new ApplicationPermission { Name = Permission.Ticket.Read },
                new ApplicationPermission { Name = Permission.Ticket.Update },
                new ApplicationPermission { Name = Permission.Ticket.Delete },
                new ApplicationPermission { Name = Permission.Ticket.Print },

                // Appointment
                new ApplicationPermission { Name = Permission.Appointment.Create },
                new ApplicationPermission { Name = Permission.Appointment.ReSchedule },
                new ApplicationPermission { Name = Permission.Appointment.Read },
                new ApplicationPermission { Name = Permission.Appointment.Update },
                new ApplicationPermission { Name = Permission.Appointment.Delete },
                new ApplicationPermission { Name = Permission.Appointment.ChangeStatus },

                // Holiday
                new ApplicationPermission { Name = Permission.Holiday.Create },
                new ApplicationPermission { Name = Permission.Holiday.Read },
                new ApplicationPermission { Name = Permission.Holiday.Update },
                new ApplicationPermission { Name = Permission.Holiday.Delete },

                // TherapyCard
                new ApplicationPermission { Name = Permission.TherapyCard.Create },
                new ApplicationPermission { Name = Permission.TherapyCard.Read },
                new ApplicationPermission { Name = Permission.TherapyCard.Update },
                new ApplicationPermission { Name = Permission.TherapyCard.Delete },
                new ApplicationPermission { Name = Permission.TherapyCard.Renew },
                new ApplicationPermission { Name = Permission.TherapyCard.CreateSession },

                // RepairCard
                new ApplicationPermission { Name = Permission.RepairCard.Create },
                new ApplicationPermission { Name = Permission.RepairCard.Read },
                new ApplicationPermission { Name = Permission.RepairCard.Update },
                new ApplicationPermission { Name = Permission.RepairCard.Delete },
                new ApplicationPermission { Name = Permission.RepairCard.ChangeStatus },
                new ApplicationPermission { Name = Permission.RepairCard.AssignToTechnician },
                new ApplicationPermission { Name = Permission.RepairCard.CreateDeliveryTime },

                // IndustrialPart
                new ApplicationPermission { Name = Permission.IndustrialPart.Create },
                new ApplicationPermission { Name = Permission.IndustrialPart.Read },
                new ApplicationPermission { Name = Permission.IndustrialPart.Update },
                new ApplicationPermission { Name = Permission.IndustrialPart.Delete },

                // MedicalProgram
                new ApplicationPermission { Name = Permission.MedicalProgram.Create },
                new ApplicationPermission { Name = Permission.MedicalProgram.Read },
                new ApplicationPermission { Name = Permission.MedicalProgram.Update },
                new ApplicationPermission { Name = Permission.MedicalProgram.Delete },

                // Department
                new ApplicationPermission { Name = Permission.Department.Create },
                new ApplicationPermission { Name = Permission.Department.Read },
                new ApplicationPermission { Name = Permission.Department.Update },
                new ApplicationPermission { Name = Permission.Department.Delete },

                // Section
                new ApplicationPermission { Name = Permission.Section.Create },
                new ApplicationPermission { Name = Permission.Section.Read },
                new ApplicationPermission { Name = Permission.Section.Update },
                new ApplicationPermission { Name = Permission.Section.Delete },

                // Room
                new ApplicationPermission { Name = Permission.Room.Create },
                new ApplicationPermission { Name = Permission.Room.Read },
                new ApplicationPermission { Name = Permission.Room.Update },
                new ApplicationPermission { Name = Permission.Room.Delete },

                // Payment
                new ApplicationPermission { Name = Permission.Payment.Create },
                new ApplicationPermission { Name = Permission.Payment.Read },
                new ApplicationPermission { Name = Permission.Payment.Update },
                new ApplicationPermission { Name = Permission.Payment.Delete },

                // Doctor
                new ApplicationPermission { Name = Permission.Doctor.Create },
                new ApplicationPermission { Name = Permission.Doctor.Read },
                new ApplicationPermission { Name = Permission.Doctor.Update },
                new ApplicationPermission { Name = Permission.Doctor.Delete },
                new ApplicationPermission { Name = Permission.Doctor.AssignDoctorToSection },
                new ApplicationPermission { Name = Permission.Doctor.AssignDoctorToSectionAndRoom },
                new ApplicationPermission { Name = Permission.Doctor.ChangeDoctorDepartment },
                new ApplicationPermission { Name = Permission.Doctor.EndDoctorAssignment },

                // Patient
                new ApplicationPermission { Name = Permission.Patient.Create },
                new ApplicationPermission { Name = Permission.Patient.Read },
                new ApplicationPermission { Name = Permission.Patient.Update },
                new ApplicationPermission { Name = Permission.Patient.Delete },

                // DisabledCard
                new ApplicationPermission { Name = Permission.DisabledCard.Create },
                new ApplicationPermission { Name = Permission.DisabledCard.Read },
                new ApplicationPermission { Name = Permission.DisabledCard.Update },
                new ApplicationPermission { Name = Permission.DisabledCard.Delete },

                // Sale
                new ApplicationPermission { Name = Permission.Sale.Create },
                new ApplicationPermission { Name = Permission.Sale.Read },
                new ApplicationPermission { Name = Permission.Sale.Update },
                new ApplicationPermission { Name = Permission.Sale.Delete },
                new ApplicationPermission { Name = Permission.Sale.Cancel },

                // User
                new ApplicationPermission { Name = Permission.User.Create },
                new ApplicationPermission { Name = Permission.User.Read },
                new ApplicationPermission { Name = Permission.User.Update },
                new ApplicationPermission { Name = Permission.User.Delete },
                new ApplicationPermission { Name = Permission.User.GrantPermissions },
                new ApplicationPermission { Name = Permission.User.DenyPermissions },
                new ApplicationPermission { Name = Permission.User.RemovePermissionOverrides },
                new ApplicationPermission { Name = Permission.User.AssignRoles },
                new ApplicationPermission { Name = Permission.User.RemoveRoles },

                // Role
                new ApplicationPermission { Name = Permission.Role.Create },
                new ApplicationPermission { Name = Permission.Role.Read },
                new ApplicationPermission { Name = Permission.Role.Update },
                new ApplicationPermission { Name = Permission.Role.Delete },
                new ApplicationPermission { Name = Permission.Role.AssignPermissions },
                new ApplicationPermission { Name = Permission.Role.RemovePermissions }
            );
        }

        if (! await _context.ReportDomains.AnyAsync())
        {
            var reportDomain = new ReportDomain
            {
                Id = 1,
                Name = "تقرير المرضى",
                RootTable = "Patients"
            };
            _context.ReportDomains.Add(reportDomain);
            await _context.SaveChangesAsync();

            var reportFields = new List<ReportField>
            {
                new ReportField
                {
                    DomainId = reportDomain.Id,
                    FieldKey = "patient_id",
                    DisplayName = "معرف المريض",
                    TableName = "Patients",
                    ColumnName = "PatientId",
                    DataType = "int",
                    IsFilterable = true
                },
                new ReportField
                {
                    DomainId = reportDomain.Id,
                    FieldKey = "patient_type",
                    DisplayName = "نوع المريض",
                    TableName = "Patients",
                    ColumnName = "PatientType",
                    DataType = "nvarchar(50)",
                    IsFilterable = true
                },
                new ReportField
                {
                    DomainId = reportDomain.Id,
                    FieldKey = "created_at_utc",
                    DisplayName = "تاريخ الإنشاء",
                    TableName = "Patients",
                    ColumnName = "CreatedAtUtc",
                    DataType = "datetimeoffset(7)",
                    IsFilterable = true
                },
                new ReportField
                {
                    DomainId = reportDomain.Id,
                    FieldKey = "patient_name",
                    DisplayName = "اسم المريض",
                    TableName = "People",
                    ColumnName = "FullName",
                    DataType = "nvarchar(200)",
                    IsFilterable = false
                },
                new ReportField
                {
                    DomainId = reportDomain.Id,
                    FieldKey = "patient_phone",
                    DisplayName = "هاتف المريض",
                    TableName = "People",
                    ColumnName = "Phone",
                    DataType = "nvarchar(15)",
                    IsFilterable = false
                }
            };

            _context.ReportFields.AddRange(reportFields);

            var reportJoins = new List<ReportJoin>
            {
                new ReportJoin
                {
                    DomainId = reportDomain.Id,
                    FromTable = "Patients",
                    ToTable = "People",
                    JoinType = "INNER",
                    JoinCondition = "Patients.PersonId = People.Id"
                }
            };
            _context.ReportJoins.AddRange(reportJoins);
        }
                
        await _context.SaveChangesAsync();
    }
}

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<AlatrafClinicDbContextInitialiser>();

        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}