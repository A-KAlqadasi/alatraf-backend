using AlatrafClinic.Domain.Departments;
using AlatrafClinic.Domain.Departments.Sections;
using AlatrafClinic.Domain.Departments.Sections.Rooms;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.InjuryReasons;
using AlatrafClinic.Domain.Diagnosises.InjurySides;
using AlatrafClinic.Domain.Diagnosises.InjuryTypes;
using AlatrafClinic.Domain.DisabledCards;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.Patients.Enums;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.Payments.DisabledPayments;
using AlatrafClinic.Domain.Payments.PatientPayments;
using AlatrafClinic.Domain.Payments.WoundedPayments;
using AlatrafClinic.Domain.People;
using AlatrafClinic.Domain.RepairCards.IndustrialParts;
using AlatrafClinic.Domain.Services;
using AlatrafClinic.Domain.Services.Appointments;
using AlatrafClinic.Domain.Services.Appointments.Holidays;
using AlatrafClinic.Domain.Services.Enums;
using AlatrafClinic.Domain.TherapyCards;
using AlatrafClinic.Domain.TherapyCards.Enums;
using AlatrafClinic.Domain.TherapyCards.MedicalPrograms;
using AlatrafClinic.Domain.WoundedCards;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data;

public static class ApplicationDbContextSeed
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        SeedInjuryLookups(modelBuilder);
        SeedDepartmentsSectionsRooms(modelBuilder);
        SeedMedicalProgramsAndServices(modelBuilder);
        SeedDiagnoses(modelBuilder);
        SeedCards(modelBuilder);
        SeedPayments(modelBuilder);
        SeedPeopleAndPatients(modelBuilder);
        SeedAppointmentsAndHolidays(modelBuilder);
        SeedIndustrialParts(modelBuilder);
    }

    private static void SeedInjuryLookups(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InjuryReason>().HasData(
            new { Id = 1, Name = "حادث مروري" },
            new { Id = 2, Name = "إصابة عمل" },
            new { Id = 3, Name = "إصابة رياضية" }
        );

        modelBuilder.Entity<InjurySide>().HasData(
            new { Id = 1, Name = "الجانب الأيسر" },
            new { Id = 2, Name = "الجانب الأيمن" },
            new { Id = 3, Name = "الجانبين" }
        );

        modelBuilder.Entity<InjuryType>().HasData(
            new { Id = 1, Name = "كسر" },
            new { Id = 2, Name = "حرق" },
            new { Id = 3, Name = "التواء" }
        );
    }

    private static void SeedDepartmentsSectionsRooms(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>().HasData(
            new { Id = 1, Name = "العلاج الطبيعي"},
            new { Id = 2, Name = "إدارة فنية" }
        );

        modelBuilder.Entity<Section>().HasData(
            new { Id = 1, Name = "القسم الاول", Code = "S-A", DepartmentId = 1 },
            new { Id = 2, Name = "القسم الثاني", Code = "S-B", DepartmentId = 1 },
            new { Id = 3, Name = "القسم الثالث", Code = "S-C", DepartmentId = 2 }
        );

        modelBuilder.Entity<Room>().HasData(
            new { Id = 1, Name = "غرفة ١٠١", SectionId = 1 },
            new { Id = 2, Name = "غرفة ١٠٢", SectionId = 1 },
            new { Id = 3, Name = "غرفة ٢٠١", SectionId = 2 }
        );
    }

    private static void SeedMedicalProgramsAndServices(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MedicalProgram>().HasData(
            new { Id = 1, Name = "برنامج آلام الظهر",  Description = "برنامج مخصص لعلاج آلام الظهر", },
            new { Id = 2, Name = "برنامج تأهيل الركبة",  Description = "برنامج مخصص لتأهيل إصابات الركبة", },
            new { Id = 3, Name = "برنامج التأهيل بعد الجراحة", Description = "برنامج تأهيلي للمرضى بعد العمليات الجراحية", }
        );

        modelBuilder.Entity<Service>().HasData(
            new { Id = 1, Name = "استشارة", Code = "SRV-CONS" },
            new { Id = 2, Name = "علاج طبيعي", Code = "SRV-THER", DepartmentId = 1 },
            new { Id = 3, Name = "اطراف صناعية",Code = "SRV-PRO", DepartmentId = 2},
            new { Id = 4, Name = "إصلاحات",Code = "SRV-REP", DepartmentId = 2},
            new { Id = 5, Name = "مبيعات",Code = "SRV-SAL", DepartmentId = 2},
            new { Id = 6, Name = "عظام",Code = "SRV-BON", DepartmentId = 1},
            new { Id = 7, Name = "أعصاب",Code = "SRV-NER", DepartmentId = 1},
            new { Id = 8, Name = "تجديد كروت علاج",Code = "SRV-REN", DepartmentId = 1},
            new { Id = 9, Name = "إصدار بدل فاقد لكرت علاج",Code = "SRV-DMG", DepartmentId = 1}

        );
    }

    private static void SeedDiagnoses(ModelBuilder modelBuilder)
{
    // -------------------------
    // DIAGNOSES
    // -------------------------
    modelBuilder.Entity<Diagnosis>().HasData(
        new
        {
            Id = 1,
            DiagnosisText = "Lower back pain due to muscle strain",
            InjuryDate = new DateTime(2025, 1, 5),
            DiagnosisType = "Acute", // stored as string
            TicketId = 1,
            PatientId = 1
        },
        new
        {
            Id = 2,
            DiagnosisText = "Right knee ligament sprain",
            InjuryDate = new DateTime(2025, 1, 12),
            DiagnosisType = "Chronic",
            TicketId = 2,
            PatientId = 2
        },
        new
        {
            Id = 3,
            DiagnosisText = "Neck pain caused by whiplash injury",
            InjuryDate = new DateTime(2025, 1, 20),
            DiagnosisType = "Acute",
            TicketId = 3,
            PatientId = 3
        }
        );
        modelBuilder.Entity("DiagnosisInjuryReasons").HasData(
            new { DiagnosesId = 1, InjuryReasonsId = 1 }, // Accident
            new { DiagnosesId = 2, InjuryReasonsId = 2 }, // Work injury
            new { DiagnosesId = 3, InjuryReasonsId = 3 }  // Sports injury
        );

        // ---------- Diagnosis ↔ InjurySide ----------
        modelBuilder.Entity("DiagnosisInjurySides").HasData(
            new { DiagnosesId = 1, InjurySidesId = 1 }, // Left side
            new { DiagnosesId = 2, InjurySidesId = 2 }, // Right side
            new { DiagnosesId = 3, InjurySidesId = 3 }  // Both
        );

        // ---------- Diagnosis ↔ InjuryType ----------
        modelBuilder.Entity("DiagnosisInjuryTypes").HasData(
            new { DiagnosesId = 1, InjuryTypesId = 1 }, // Fracture
            new { DiagnosesId = 2, InjuryTypesId = 3 }, // Sprain
            new { DiagnosesId = 3, InjuryTypesId = 2 }  // Burn / whiplash
        );
    }

    private static void SeedCards(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<TherapyCard>().HasData(
            new { Id = 1, ProgramStartDate = new DateTime(2025, 1, 1), ProgramEndDate = new DateTime(2025, 1, 20), IsActive = true, NumberOfSessions = 20, DiagnosisId = 1, Type = TherapyCardType.General, SessionPricePerType = 200, CardStatus = TherapyCardStatus.New, PatientId = 1 },
            new { Id = 2, ProgramStartDate = new DateTime(2025, 1, 15), ProgramEndDate = new DateTime(2025, 2, 5), IsActive = true, NumberOfSessions = 20, DiagnosisId = 2, Type = TherapyCardType.Special , SessionPricePerType = 2000, CardStatus = TherapyCardStatus.New, PatientId = 2 },
            new { Id = 3, ProgramStartDate = new DateTime(2025, 2, 1), ProgramEndDate = new DateTime(2025, 4, 1), IsActive = true, NumberOfSessions = 20, DiagnosisId = 3, Type = TherapyCardType.NerveKids, SessionPricePerType = 400, CardStatus = TherapyCardStatus.New , PatientId = 3 }
        );

        modelBuilder.Entity<WoundedCard>().HasData(
            new { Id = 1, CardNumber = "WC-0001", Expiration = new DateTime(2026, 1, 1), CardImagePath = (string?)null, PatientId = 1 },
            new { Id = 2, CardNumber = "WC-0002", Expiration = new DateTime(2026, 2, 1), CardImagePath = (string?)null, PatientId = 2 },
            new { Id = 3, CardNumber = "WC-0003", Expiration = new DateTime(2026, 3, 1), CardImagePath = (string?)null, PatientId = 3 }
        );

        modelBuilder.Entity<DisabledCard>().HasData(
            new { Id = 1, CardNumber = "DC-0001", ExpirationDate = new DateTime(2026, 1, 1), CardImagePath = (string?)null, PatientId = 1 },
            new { Id = 2, CardNumber = "DC-0002", ExpirationDate = new DateTime(2026, 2, 1), CardImagePath = (string?)null, PatientId = 2 },
            new { Id = 3, CardNumber = "DC-0003", ExpirationDate = new DateTime(2026, 3, 1), CardImagePath = (string?)null, PatientId = 3 }
        );
    }

    private static void SeedPayments(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>().HasData(
            new
            {
                Id = 10,
                TotalAmount = 200m,
                PaidAmount = 200m,
                Discount = (decimal?)null,
                PaymentDate = new DateTime(2025, 1, 5),
                IsCompleted = true,
                Notes = "دفع كامل مقابل جلسة علاج",
                AccountKind = AccountKind.Patient,                // stored as string
                PaymentReference = PaymentReference.TherapyCardNew, // stored as string
                DiagnosisId = 1,
                TicketId = 1
            },
            new
            {
                Id = 11,
                TotalAmount = 300m,
                PaidAmount = 250m,
                Discount = 50m,
                PaymentDate = new DateTime(2025, 1, 10),
                IsCompleted = false,
                Notes = "دفع جزئي مع خصم",
                AccountKind = AccountKind.Wounded,
                PaymentReference = PaymentReference.TherapyCardRenew,
                DiagnosisId = 2,
                TicketId = 2
            },
            new
            {
                Id = 12,
                TotalAmount = 150m,
                PaidAmount = (decimal?)null,  // unpaid
                Discount = (decimal?)null,
                PaymentDate = (DateTime?)null,
                IsCompleted = false,
                Notes = "لم يتم الدفع بعد",
                AccountKind = AccountKind.Disabled,
                PaymentReference = PaymentReference.Sales,
                DiagnosisId = 3,
                TicketId = 3
            }
        );

        modelBuilder.Entity<PatientPayment>().HasData(
            new { Id = 10, PaymentId = 1, PatientId = 1 }
        );
        
        modelBuilder.Entity<WoundedPayment>().HasData(
            new { Id = 11, PaymentId = 1, WoundedCardId = 1 }
        );

        // modelBuilder.Entity<DisabledPayment>().HasData(
        //     new { Id = 12, PaymentId = 1, DisabledCardId = 1 }
        // );
    }
    private static void SeedPeopleAndPatients(ModelBuilder modelBuilder)
    {
    // الأشخاص (People)
    modelBuilder.Entity<Person>().HasData(
        new
        {
            Id = 1,
            FullName = "علي أحمد",
            Birthdate = new DateTime(1990, 1, 1),
            Phone = "771234567",
            NationalNo = "NAT-0001",
            Gender = true,                 // ذكر
            Address = "صنعاء",
            // audit fields (if exist on AuditableEntity)
            CreatedAtUtc = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
            LastModifiedUtc = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
            CreatedBy = "Seed",
            LastModifiedBy = "Seed",
            IsDeleted = false
        },
        new
        {
            Id = 2,
            FullName = "محمد صالح",
            Birthdate = new DateTime(1985, 5, 10),
            Phone = "781234568",
            NationalNo = "NAT-0002",
            Gender = true,
            Address = "عدن",
            CreatedAtUtc = new DateTimeOffset(2025, 1, 2, 0, 0, 0, TimeSpan.Zero),
            LastModifiedUtc = new DateTimeOffset(2025, 1, 2, 0, 0, 0, TimeSpan.Zero),
            CreatedBy = "Seed",
            LastModifiedBy = "Seed",
            IsDeleted = false
        },
        new
        {
            Id = 3,
            FullName = "سارة علي",
            Birthdate = new DateTime(1995, 3, 20),
            Phone = "731234569",
            NationalNo = "NAT-0003",
            Gender = false,               // أنثى
            Address = "تعز",
            CreatedAtUtc = new DateTimeOffset(2025, 1, 3, 0, 0, 0, TimeSpan.Zero),
            LastModifiedUtc = new DateTimeOffset(2025, 1, 3, 0, 0, 0, TimeSpan.Zero),
            CreatedBy = "Seed",
            LastModifiedBy = "Seed",
            IsDeleted = false
        }
    );

    // لاحظ: لم نضع AutoRegistrationNumber في الـ seed
    // لأنه عمود محسوب (Computed Column) وسيتم توليده في SQL
    // بالشكل: سنة_شهر_يوم_معرّف_الشخص
    
    modelBuilder.Entity<Patient>().HasData(
        new
        {
            Id = 1,
            PersonId = 1,
            PatientType = PatientType.Normal,  // عادي
            CreatedAtUtc = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
            LastModifiedUtc = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
            CreatedBy = "Seed",
            LastModifiedBy = "Seed",
            IsDeleted = false
        },
        new
        {
            Id = 2,
            PersonId = 2,
            PatientType = PatientType.Wounded,
            CreatedAtUtc = new DateTimeOffset(2025, 1, 2, 0, 0, 0, TimeSpan.Zero),
            LastModifiedUtc = new DateTimeOffset(2025, 1, 2, 0, 0, 0, TimeSpan.Zero),
            CreatedBy = "Seed",
            LastModifiedBy = "Seed",
            IsDeleted = false
        },
        new
        {
            Id = 3,
            PersonId = 3,
            PatientType = PatientType.Disabled,
            CreatedAtUtc = new DateTimeOffset(2025, 1, 3, 0, 0, 0, TimeSpan.Zero),
            LastModifiedUtc = new DateTimeOffset(2025, 1, 3, 0, 0, 0, TimeSpan.Zero),
            CreatedBy = "Seed",
            LastModifiedBy = "Seed",
            IsDeleted = false
            }
        );
    }

    private static void SeedAppointmentsAndHolidays(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>().HasData(
        new
        {
            Id = 1,
            TicketId = 1,
            PatientType = PatientType.Normal,      // stored as string
            AttendDate = new DateTime(2025, 1, 10),
            Status = AppointmentStatus.Scheduled,  // stored as string
            Notes = "موعد متابعة للمريض",
            CreatedAtUtc = DateTimeOffset.UtcNow,
            LastModifiedUtc = DateTimeOffset.UtcNow,
            CreatedBy = "Seed",
            LastModifiedBy = "Seed",
            IsDeleted = false
        },
        new
        {
            Id = 2,
            TicketId = 2,
            PatientType = PatientType.Wounded,
            AttendDate = new DateTime(2025, 1, 11),
            Status = AppointmentStatus.Today,
            Notes = "موعد طارئ",
            CreatedAtUtc = DateTimeOffset.UtcNow,
            LastModifiedUtc = DateTimeOffset.UtcNow,
            CreatedBy = "Seed",
            LastModifiedBy = "Seed",
            IsDeleted = false
        },
        new
        {
            Id = 3,
            TicketId = 3,
            PatientType = PatientType.Normal,
            AttendDate = new DateTime(2025, 1, 12),
            Status = AppointmentStatus.Attended,
            Notes = "حضر الموعد في الوقت المحدد",
            CreatedAtUtc = DateTimeOffset.UtcNow,
            LastModifiedUtc = DateTimeOffset.UtcNow,
            CreatedBy = "Seed",
            LastModifiedBy = "Seed",
            IsDeleted = false
        }
        );

        modelBuilder.Entity<Holiday>().HasData(
        new
        {
            Id = 3,
            Name = "عيد العمال العالمي",
            StartDate = new DateTime(1, 5, 1),
            EndDate = (DateTime?)null,
            IsRecurring = true,
            IsActive = true,
            Type = HolidayType.Fixed
        },
        new
        {
            Id = 4,
            Name = "عيد الوحدة اليمنية",
            StartDate = new DateTime(1, 5, 22),
            EndDate = (DateTime?)null,
            IsRecurring = true,
            IsActive = true,
            Type = HolidayType.Fixed
        },
        new
        {
            Id = 5,
            Name = "ثورة 26 سبتمبر",
            StartDate = new DateTime(1, 9, 26),
            EndDate = (DateTime?)null,
            IsRecurring = true,
            IsActive = true,
            Type = HolidayType.Fixed
        },
        new
        {
            Id = 6,
            Name = "ثورة 14 أكتوبر",
            StartDate = new DateTime(1, 10, 14),
            EndDate = (DateTime?)null,
            IsRecurring = true,
            IsActive = true,
            Type = HolidayType.Fixed
        },
        new
        {
            Id = 7,
            Name = "عيد الجلاء",
            StartDate = new DateTime(1, 11, 30),
            EndDate = (DateTime?)null,
            IsRecurring = true,
            IsActive = true,
            Type = HolidayType.Fixed
        }
        );
    }
    private static void SeedIndustrialParts(ModelBuilder modelBuilder)
{
    // --------------------------------------------------------------------
    // INDUSTRIAL PARTS (قطع صناعية)
    // --------------------------------------------------------------------
    modelBuilder.Entity<IndustrialPart>().HasData(
        new
        {
            Id = 1,
            Name = "دعامة الركبة",
            Description = "تستخدم لتثبيت ودعم مفصل الركبة",
            CreatedAtUtc = DateTimeOffset.UtcNow,
            LastModifiedUtc = DateTimeOffset.UtcNow,
            CreatedBy = "Seed",
            LastModifiedBy = "Seed",
            IsDeleted = false
        },
        new
        {
            Id = 2,
            Name = "حزام الظهر الطبي",
            Description = "يساعد على دعم أسفل الظهر وتخفيف الألم",
            CreatedAtUtc = DateTimeOffset.UtcNow,
            LastModifiedUtc = DateTimeOffset.UtcNow,
            CreatedBy = "Seed",
            LastModifiedBy = "Seed",
            IsDeleted = false
        },
        new
        {
            Id = 3,
            Name = " كولار رقبة طبية",
            Description = "تستخدم لتثبيت الرقبة في حالات الإصابات",
            CreatedAtUtc = DateTimeOffset.UtcNow,
            LastModifiedUtc = DateTimeOffset.UtcNow,
            CreatedBy = "Seed",
            LastModifiedBy = "Seed",
            IsDeleted = false
        }
    );

    // --------------------------------------------------------------------
    // INDUSTRIAL PART UNITS (سعر الوحدة لكل قطعة صناعية)
    // Requires: UnitId existing (1,2,3)
    // --------------------------------------------------------------------
    modelBuilder.Entity<IndustrialPartUnit>().HasData(
        new
        {
            Id = 1,
            IndustrialPartId = 1, // دعامة الركبة
            UnitId = 1,            // قطعة
            PricePerUnit = 80m,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            LastModifiedUtc = DateTimeOffset.UtcNow,
            CreatedBy = "Seed",
            LastModifiedBy = "Seed",
            IsDeleted = false
        },
        new
        {
            Id = 2,
            IndustrialPartId = 2, // حزام الظهر
            UnitId = 1,            // قطعة
            PricePerUnit = 120m,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            LastModifiedUtc = DateTimeOffset.UtcNow,
            CreatedBy = "Seed",
            LastModifiedBy = "Seed",
            IsDeleted = false
        },
        new
        {
            Id = 3,
            IndustrialPartId = 3, // رقبة طبية
            UnitId = 1,            // قطعة
            PricePerUnit = 90m,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            LastModifiedUtc = DateTimeOffset.UtcNow,
            CreatedBy = "Seed",
            LastModifiedBy = "Seed",
            IsDeleted = false
        }
        );
    }
}