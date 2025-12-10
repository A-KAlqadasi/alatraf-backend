using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Departments;
using AlatrafClinic.Domain.Departments.DoctorSectionRooms;
using AlatrafClinic.Domain.Departments.Sections;
using AlatrafClinic.Domain.Departments.Sections.Rooms;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.DiagnosisIndustrialParts;
using AlatrafClinic.Domain.Diagnosises.DiagnosisPrograms;
using AlatrafClinic.Domain.Diagnosises.Enums;
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
using AlatrafClinic.Domain.Patients.Enums;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.Payments.PatientPayments;
using AlatrafClinic.Domain.Payments.WoundedPayments;
using AlatrafClinic.Domain.People;
using AlatrafClinic.Domain.People.Doctors;
using AlatrafClinic.Domain.RepairCards;
using AlatrafClinic.Domain.RepairCards.Enums;
using AlatrafClinic.Domain.RepairCards.IndustrialParts;
using AlatrafClinic.Domain.Services;
using AlatrafClinic.Domain.Services.Appointments;
using AlatrafClinic.Domain.Services.Appointments.Holidays;
using AlatrafClinic.Domain.Services.Enums;
using AlatrafClinic.Domain.Services.Tickets;
using AlatrafClinic.Domain.TherapyCards;
using AlatrafClinic.Domain.TherapyCards.Enums;
using AlatrafClinic.Domain.TherapyCards.MedicalPrograms;
using AlatrafClinic.Domain.TherapyCards.Sessions;
using AlatrafClinic.Domain.TherapyCards.TherapyCardTypePrices;
using AlatrafClinic.Domain.WoundedCards;
using AlatrafClinic.Infrastructure.Identity;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data;

public static class AlatrafClinicDbContextInitializer
{
    private static readonly DateTimeOffset  SeedTimestamp =
       new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private static readonly DateTime SeedDate = new DateTime(2025, 1, 1);

    public static void Seed(ModelBuilder modelBuilder)
    {
        SeedInjuryLookups(modelBuilder);
        SeedDepartmentsSectionsRooms(modelBuilder);
        SeedMedicalProgramsAndServices(modelBuilder);
        SeedDiagnoses(modelBuilder);
        SeedDiagnosisIndustrialParts(modelBuilder);
        SeedDiagnosisPrograms(modelBuilder);
        SeedCards(modelBuilder);
        SeedPayments(modelBuilder);
        SeedPeopleAndPatients(modelBuilder);
        SeedAppointmentsAndHolidays(modelBuilder);
        SeedIndustrialParts(modelBuilder);
        SeedUnits(modelBuilder);
        // Inventory seeds
        SeedSuppliers(modelBuilder);
        SeedItemsAndUnits(modelBuilder);
        SeedStoresAndStoreItemUnits(modelBuilder);
        SeedPurchaseInvoices(modelBuilder);
        SeedExchangeOrders(modelBuilder);
        SeedTickets(modelBuilder);
        SeedTherapyCards(modelBuilder);
        SeedSessions(modelBuilder);
        SeedRepairCards(modelBuilder);
        SeedRoles(modelBuilder);
        SeedApplicationPermissions(modelBuilder);
        SeedAdminRolePermissions(modelBuilder);
        SeedAdminUser(modelBuilder);
        SeedTherapyCardTypePrices(modelBuilder);
        SeedDoctors(modelBuilder);
        SeedDoctorSectionRooms(modelBuilder);
    }

    private static void SeedInjuryLookups(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InjuryReason>().HasData(
            new
            {
                Id = 1,
                Name = "حادث مروري",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },

            new
            {
                Id = 2,
                Name = "إصابة عمل",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 3,
                Name = "إصابة رياضية",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
        );

        modelBuilder.Entity<InjurySide>().HasData(
            new
            {
                Id = 1,
                Name = "الجانب الأيسر",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 2,
                Name = "الجانب الأيمن",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 3,
                Name = "الجانبين",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
        );

        modelBuilder.Entity<InjuryType>().HasData(
            new
            {
                Id = 1,
                Name = "كسر",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 2,
                Name = "حرق",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 3,
                Name = "التواء",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
        );
    }

    private static void SeedDepartmentsSectionsRooms(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>().HasData(
            new
            {
                Id = 1,
                Name = "العلاج الطبيعي",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 2,
                Name = "إدارة فنية",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Section>().HasData(
            new
            {
                Id = 1,
                Name = "القسم الاول",
                Code = "S-A",
                DepartmentId = 1,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 2,
                Name = "القسم الثاني",
                Code = "S-B",
                DepartmentId = 1,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 3,
                Name = "القسم الثالث",
                Code = "S-C",
                DepartmentId = 2,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Room>().HasData(
            new
            {
                Id = 1,
                Name = "غرفة ١٠١",
                SectionId = 1,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 2,
                Name = "غرفة ١٠٢",
                SectionId = 1,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 3,
                Name = "غرفة ٢٠١",
                SectionId = 2,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
        );
    }

    private static void SeedMedicalProgramsAndServices(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MedicalProgram>().HasData(
            new
            {
                Id = 1,
                Name = "برنامج آلام الظهر",
                Description = "برنامج مخصص لعلاج آلام الظهر",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 2,
                Name = "برنامج تأهيل الركبة",
                Description = "برنامج مخصص لتأهيل إصابات الركبة",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 3,
                Name = "برنامج التأهيل بعد الجراحة",
                Description = "برنامج تأهيلي للمرضى بعد العمليات الجراحية",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
        );

        modelBuilder.Entity<Service>().HasData(
            new
            {
                Id = 1,
                Name = "استشارة",
                Code = "SRV-CONS",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 2,
                Name = "علاج طبيعي",
                Code = "SRV-THER",
                DepartmentId = 1,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 3,
                Name = "اطراف صناعية",
                Code = "SRV-PRO",
                DepartmentId = 2,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 4,
                Name = "مبيعات",
                Code = "SRV-SAL",
                DepartmentId = 2,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 5,
                Name = "إصلاحات",
                Code = "SRV-REP",
                DepartmentId = 2,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 6,
                Name = "عظام",
                Code = "SRV-BON",
                DepartmentId = 1,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 7,
                Name = "أعصاب",
                Code = "SRV-NER",
                DepartmentId = 1,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 8,
                Name = "تجديد كروت علاج",
                Code = "SRV-REN",
                DepartmentId = 1,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 9,
                Name = "إصدار بدل فاقد لكرت علاج",
                Code = "SRV-DMG",
                DepartmentId = 1,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }

        );
    }
    private static void SeedTickets(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>().HasData(
            new
            {
                Id = 1,
                PatientId = 1,
                ServiceId = 2,
                Status = TicketStatus.New,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 2,
                PatientId = 1,
                ServiceId = 3,
                Status = TicketStatus.Pause,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 3,
                PatientId = 2,
                ServiceId = 5,
                Status = TicketStatus.New,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
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
                InjuryDate = SeedDate,
                DiagnoType = DiagnosisType.Limbs, // stored as string
                TicketId = 1,
                PatientId = 1,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 2,
                DiagnosisText = "Right knee ligament sprain",
                InjuryDate = SeedDate,
                DiagnoType = DiagnosisType.Therapy,
                TicketId = 2,
                PatientId = 2,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 3,
                DiagnosisText = "Neck pain caused by whiplash injury",
                InjuryDate = SeedDate,
                DiagnoType = DiagnosisType.Sales,
                TicketId = 3,
                PatientId = 3,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
            );
        // modelBuilder.Entity("DiagnosisInjuryReasons").HasData(
        //     new { DiagnosesId = 1, InjuryReasonsId = 1 }, // Accident
        //     new { DiagnosesId = 2, InjuryReasonsId = 2 }, // Work injury
        //     new { DiagnosesId = 3, InjuryReasonsId = 3 }  // Sports injury
        // );

        // // ---------- Diagnosis ↔ InjurySide ----------
        // modelBuilder.Entity("DiagnosisInjurySides").HasData(
        //     new { DiagnosesId = 1, InjurySidesId = 1 }, // Left side
        //     new { DiagnosesId = 2, InjurySidesId = 2 }, // Right side
        //     new { DiagnosesId = 3, InjurySidesId = 3 }  // Both
        // );

        // // ---------- Diagnosis ↔ InjuryType ----------
        // modelBuilder.Entity("DiagnosisInjuryTypes").HasData(
        //     new { DiagnosesId = 1, InjuryTypesId = 1 }, // Fracture
        //     new { DiagnosesId = 2, InjuryTypesId = 3 }, // Sprain
        //     new { DiagnosesId = 3, InjuryTypesId = 2 }  // Burn / whiplash
        // );
    }
    private static void SeedDiagnosisIndustrialParts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DiagnosisIndustrialPart>().HasData(
            new
            {
                Id = 1,
                DiagnosisId = 1,
                IndustrialPartUnitId = 1,       // دعامة الركبة - قطعة
                Price = 80m,
                Quantity = 1,
                DoctorAssignDate = SeedDate,
                DoctorSectionRoomId = (int?)null,
                RepairCardId = 1,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 2,
                DiagnosisId = 1,
                IndustrialPartUnitId = 2,       // حزام الظهر
                Price = 120m,
                Quantity = 1,
                DoctorAssignDate = SeedDate,
                DoctorSectionRoomId = (int?)null,
                RepairCardId = 1,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 3,
                DiagnosisId = 1,
                IndustrialPartUnitId = 3,       // رقبة طبية
                Price = 90m,
                Quantity = 1,
                DoctorAssignDate = SeedDate,
                DoctorSectionRoomId = (int?)null,
                RepairCardId = 1,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
        );
    }

    private static void SeedDiagnosisPrograms(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DiagnosisProgram>().HasData(
            new
            {
                Id = 1,
                DiagnosisId = 2,
                MedicalProgramId = 1,    // برنامج آلام الظهر
                Duration = 10,
                Notes = "خطة علاج لآلام أسفل الظهر لمدة عشرة جلسات",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 2,
                DiagnosisId = 2,
                MedicalProgramId = 2,    // برنامج تأهيل الركبة
                Duration = 8,
                Notes = "برنامج تأهيل للركبة بعد إصابة رياضية",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 3,
                DiagnosisId = 2,
                MedicalProgramId = 3,    // برنامج التأهيل بعد الجراحة
                Duration = 12,
                Notes = "تأهيل للرقبة والكتف بعد إصابة حادة",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
        );
    }



    private static void SeedCards(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<WoundedCard>().HasData(
            new
            {
                Id = 1,
                CardNumber = "WC-0001",
                Expiration = SeedDate.AddDays(100),
                CardImagePath = (string?)null,
                PatientId = 1,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 2,
                CardNumber = "WC-0002",
                Expiration = SeedDate.AddDays(100),
                CardImagePath = (string?)null,
                PatientId = 2,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 3,
                CardNumber = "WC-0003",
                Expiration = SeedDate.AddDays(100),
                CardImagePath = (string?)null,
                PatientId = 3,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
        );

        modelBuilder.Entity<DisabledCard>().HasData(
            new
            {
                Id = 1,
                CardNumber = "DC-0001",
                ExpirationDate = SeedDate.AddDays(100),
                CardImagePath = (string?)null,
                PatientId = 1,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 2,
                CardNumber = "DC-0002",
                ExpirationDate = SeedDate.AddDays(100),
                CardImagePath = (string?)null,
                PatientId = 2,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 3,
                CardNumber = "DC-0003",
                ExpirationDate = SeedDate.AddDays(100),
                CardImagePath = (string?)null,
                PatientId = 3,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
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
                PaymentDate = SeedDate,
                IsCompleted = true,
                Notes = "دفع كامل مقابل جلسة علاج",
                AccountKind = AccountKind.Patient,                // stored as string
                PaymentReference = PaymentReference.TherapyCardNew, // stored as string
                DiagnosisId = 1,
                TicketId = 1,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 11,
                TotalAmount = 300m,
                PaidAmount = 250m,
                Discount = 50m,
                PaymentDate = SeedDate,
                IsCompleted = false,
                Notes = "دفع جزئي مع خصم",
                AccountKind = AccountKind.Wounded,
                PaymentReference = PaymentReference.TherapyCardRenew,
                DiagnosisId = 2,
                TicketId = 2,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
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
                TicketId = 3,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
        );

        modelBuilder.Entity<PatientPayment>().HasData(
            new
            {
                Id = 10,
                PaymentId = 1,
                VoucherNumber = "VOU-0001",
                PatientId = 1,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
        );

        modelBuilder.Entity<WoundedPayment>().HasData(
            new
            {
                Id = 11,
                PaymentId = 1,
                WoundedCardId = 1,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
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
                Birthdate = SeedDate,
                Phone = "771234567",
                NationalNo = "NAT-0001",
                Gender = true,                 // ذكر
                Address = "صنعاء",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 2,
                FullName = "محمد صالح",
                Birthdate = SeedDate,
                Phone = "781234568",
                NationalNo = "NAT-0002",
                Gender = true,
                Address = "عدن",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 3,
                FullName = "سارة علي",
                Birthdate = SeedDate,
                Phone = "731234569",
                NationalNo = "NAT-0003",
                Gender = false,               // أنثى
                Address = "تعز",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 4,
                FullName = "عبدالكريم شوقي يوسف أحمد",
                Birthdate = SeedDate,
                Phone = "782422822",
                NationalNo = "NAT-0004",
                Gender = true,               // ذكر
                Address = "تعز",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
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
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 2,
                PersonId = 2,
                PatientType = PatientType.Wounded,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 3,
                PersonId = 3,
                PatientType = PatientType.Disabled,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
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
            TicketId = 2,
            PatientType = PatientType.Normal,      // stored as string
            AttendDate = SeedDate,
            Status = AppointmentStatus.Scheduled,  // stored as string
            Notes = "موعد متابعة للمريض",
            CreatedAtUtc = SeedTimestamp,
            LastModifiedUtc = SeedTimestamp,
            CreatedBy = "Seed",
            LastModifiedBy = "Seed",
            IsDeleted = false
        }
        // ,
        // new
        // {
        //     Id = 2,
        //     TicketId = 2,
        //     PatientType = PatientType.Wounded,
        //     AttendDate = new DateTime(2025, 1, 11),
        //     Status = AppointmentStatus.Today,
        //     Notes = "موعد طارئ",
        //     CreatedAtUtc = SeedTimestamp,
        //     LastModifiedUtc = SeedTimestamp,
        //     CreatedBy = "Seed",
        //     LastModifiedBy = "Seed",
        //     IsDeleted = false
        // },
        // new
        // {
        //     Id = 3,
        //     TicketId = 3,
        //     PatientType = PatientType.Normal,
        //     AttendDate = new DateTime(2025, 1, 12),
        //     Status = AppointmentStatus.Attended,
        //     Notes = "حضر الموعد في الوقت المحدد",
        //     CreatedAtUtc = SeedTimestamp,
        //     LastModifiedUtc = SeedTimestamp,
        //     CreatedBy = "Seed",
        //     LastModifiedBy = "Seed",
        //     IsDeleted = false,

        // }
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
            Type = HolidayType.Fixed,
            CreatedAtUtc = SeedTimestamp,
            LastModifiedUtc = SeedTimestamp,
            CreatedBy = "Seed",
            LastModifiedBy = "Seed",
            IsDeleted = false
        },
        new
        {
            Id = 4,
            Name = "عيد الوحدة اليمنية",
            StartDate = new DateTime(1, 5, 22),
            EndDate = (DateTime?)null,
            IsRecurring = true,
            IsActive = true,
            Type = HolidayType.Fixed,
            CreatedAtUtc = SeedTimestamp,
            LastModifiedUtc = SeedTimestamp,
            CreatedBy = "Seed",
            LastModifiedBy = "Seed",
            IsDeleted = false
        },
        new
        {
            Id = 5,
            Name = "ثورة 26 سبتمبر",
            StartDate = new DateTime(1, 9, 26),
            EndDate = (DateTime?)null,
            IsRecurring = true,
            IsActive = true,
            Type = HolidayType.Fixed,
            CreatedAtUtc = SeedTimestamp,
            LastModifiedUtc = SeedTimestamp,
            CreatedBy = "Seed",
            LastModifiedBy = "Seed",
            IsDeleted = false
        },
        new
        {
            Id = 6,
            Name = "ثورة 14 أكتوبر",
            StartDate = new DateTime(1, 10, 14),
            EndDate = (DateTime?)null,
            IsRecurring = true,
            IsActive = true,
            Type = HolidayType.Fixed,
            CreatedAtUtc = SeedTimestamp,
            LastModifiedUtc = SeedTimestamp,
            CreatedBy = "Seed",
            LastModifiedBy = "Seed",
            IsDeleted = false
        },
        new
        {
            Id = 7,
            Name = "عيد الجلاء",
            StartDate = new DateTime(1, 11, 30),
            EndDate = (DateTime?)null,
            IsRecurring = true,
            IsActive = true,
            Type = HolidayType.Fixed,
            CreatedAtUtc = SeedTimestamp,
            LastModifiedUtc = SeedTimestamp,
            CreatedBy = "Seed",
            LastModifiedBy = "Seed",
            IsDeleted = false
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
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 2,
                Name = "حزام الظهر الطبي",
                Description = "يساعد على دعم أسفل الظهر وتخفيف الألم",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 3,
                Name = " كولار رقبة طبية",
                Description = "تستخدم لتثبيت الرقبة في حالات الإصابات",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
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
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
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
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
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
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
        );
    }

    private static void SeedUnits(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GeneralUnit>().HasData(
            new
            {
                Id = 1,
                Name = "قطعة",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 2,
                Name = "زوج",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 3,
                Name = "يمين",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 4,
                Name = "يسار",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
        );
    }

    private static void SeedSuppliers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Supplier>().HasData(
            new
            {
                Id = 1,
                SupplierName = "Default Supplier",
                Phone = "0000000000",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
        );
    }

    private static void SeedItemsAndUnits(ModelBuilder modelBuilder)
    {
        // Items
        modelBuilder.Entity<Item>().HasData(
            new
            {
                Id = 1,
                Name = "Sample Item A",
                Description = "Sample inventory item A",
                BaseUnitId = 1,
                IsActive = true,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
        );

        // ItemUnits as independent entity
        modelBuilder.Entity<ItemUnit>().HasData(
            new
            {
                Id = 1,
                ItemId = 1,
                UnitId = 1,
                Price = 100m,
                ConversionFactor = 1m,
                MinPriceToPay = (decimal?)null,
                MaxPriceToPay = (decimal?)null,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 2,
                ItemId = 1,
                UnitId = 2,
                Price = 180m,
                ConversionFactor = 2m,
                MinPriceToPay = (decimal?)null,
                MaxPriceToPay = (decimal?)null,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
        );
    }

    private static void SeedStoresAndStoreItemUnits(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Store>().HasData(
     new
     {
         Id = 1,
         Name = "Main Store",
         CreatedAtUtc = SeedTimestamp,
         LastModifiedUtc = SeedTimestamp,
         CreatedBy = "Seed",
         LastModifiedBy = "Seed",
         IsDeleted = false
     }
 );

        modelBuilder.Entity<StoreItemUnit>().HasData(
            new
            {
                Id = 1,
                StoreId = 1,
                ItemUnitId = 1,
                Quantity = 100m,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
        );

    }
    private static void SeedPurchaseInvoices(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PurchaseInvoice>().HasData(
            new
            {
                Id = 1,
                Number = "PI-1001",
                Date = SeedDate,
                SupplierId = 1,
                StoreId = 1,
                Status = AlatrafClinic.Domain.Inventory.Enums.PurchaseInvoiceStatus.Draft,
                PostedAtUtc = (DateTime?)null,
                PaidAtUtc = (DateTime?)null,
                PaymentAmount = (decimal?)null,
                PaymentMethod = (string?)null,
                PaymentReference = (string?)null,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
        );

        // Seed dependent PurchaseItem directly
        modelBuilder.Entity<PurchaseItem>().HasData(
            new
            {
                Id = 1,
                PurchaseInvoiceId = 1,
                StoreItemUnitId = 1,
                Quantity = 10m,
                UnitPrice = 90m,
                Notes = (string?)null,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false  // IMPORTANT: You must include this
            }
        );
    }

    private static void SeedExchangeOrders(ModelBuilder modelBuilder)
    {

        // Seed ExchangeOrder
        modelBuilder.Entity<ExchangeOrder>().HasData(
            new
            {
                Id = 1,
                Number = "EX-1",
                IsApproved = false,
                Notes = (string?)null,
                RelatedOrderId = (int?)null,
                RelatedSaleId = (int?)null,
                StoreId = 1,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
        );

        modelBuilder.Entity<ExchangeOrderItem>().HasData(
          new
          {
              Id = 1,
              ExchangeOrderId = 1,
              StoreItemUnitId = 1,
              Quantity = 2m,
              CreatedAtUtc = SeedTimestamp,
              LastModifiedUtc = SeedTimestamp,
              CreatedBy = "Seed",
              LastModifiedBy = "Seed",
              IsDeleted = false
          }
        );
    }

    private static void SeedTherapyCardTypePrices(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TherapyCardTypePrice>().HasData(
            new
            {
                Id = 1,
                Type = TherapyCardType.General,
                SessionPrice = 200m,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 2,
                Type = TherapyCardType.Special,
                SessionPrice = 2000m,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 3,
                Type = TherapyCardType.NerveKids,
                SessionPrice = 400m,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
        );
    }

    private static void SeedTherapyCards(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TherapyCard>().HasData(
            new
            {
                Id = 1,
                DiagnosisId = 2,
                ProgramStartDate = new DateTime(2025, 1, 1),
                ProgramEndDate = new DateTime(2025, 1, 20),
                IsActive = true,
                Type = TherapyCardType.General,
                SessionPricePerType = 200m,
                CardStatus = TherapyCardStatus.New, // enum → string
                ParentCardId = (int?)null,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
            //, new
            // {
            //     Id = 2,
            //     DiagnosisId = 2,
            //     ProgramStartDate = new DateTime(2025, 1, 10),
            //     ProgramEndDate = new DateTime(2025, 2, 20),
            //     NumberOfSessions = 8,
            //     IsActive = true,
            //     Type = TherapyCardType.Special,
            //     SessionPricePerType = 2000m,
            //     CardStatus = TherapyCardStatus.New,
            //     ParentCardId = (int?)null,
            //     CreatedAtUtc = new DateTimeOffset(2025, 1, 10, 9, 0, 0, TimeSpan.Zero),
            //     LastModifiedUtc = new DateTimeOffset(2025, 1, 10, 9, 0, 0, TimeSpan.Zero),
            //     CreatedBy = "Seed",
            //     LastModifiedBy = "Seed",
            //     IsDeleted = false
            // },
            // new
            // {
            //     Id = 3,
            //     DiagnosisId = 3,
            //     ProgramStartDate = new DateTime(2025, 1, 15),
            //     ProgramEndDate = new DateTime(2025, 3, 1),
            //     NumberOfSessions = 12,
            //     IsActive = false, // مثال: بطاقة منتهية
            //     Type = TherapyCardType.General,
            //     SessionPricePerType = 400m,
            //     CardStatus = TherapyCardStatus.Renew,
            //     ParentCardId = 1,  // مثال: بطاقة مجددة من البطاقة رقم 1
            //     CreatedAtUtc = new DateTimeOffset(2025, 1, 15, 10, 0, 0, TimeSpan.Zero),
            //     LastModifiedUtc = new DateTimeOffset(2025, 1, 15, 10, 0, 0, TimeSpan.Zero),
            //     CreatedBy = "Seed",
            //     LastModifiedBy = "Seed",
            //     IsDeleted = false
            // }
        );
    }


    private static void SeedSessions(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Session>().HasData(
            new
            {
                Id = 1,
                TherapyCardId = 1,
                Number = 1,
                SessionDate = new DateTime(2025, 1, 10),
                IsTaken = true,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 2,
                TherapyCardId = 1,
                Number = 2,
                SessionDate = new DateTime(2025, 1, 11),
                IsTaken = false,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
            // ,new
            // {
            //     Id = 3,
            //     TherapyCardId = 1,
            //     Number = 3,
            //     SessionDate = new DateTime(2025, 1, 12),
            //     IsTaken = false,
            //     CreatedAtUtc = SeedTimestamp,
            //     LastModifiedUtc = SeedTimestamp,
            //     CreatedBy = "Seed",
            //     LastModifiedBy = "Seed",
            //     IsDeleted = false
            // }
        );
    }

    private static void SeedSessionPrograms(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SessionProgram>().HasData(
            new
            {
                Id = 1,
                DiagnosisProgramId = 1,
                SessionId = 1,
                DoctorSectionRoomId = 1,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 2,
                DiagnosisProgramId = 2,
                SessionId = 1,
                DoctorSectionRoomId = 1,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 3,
                DiagnosisProgramId = 3,
                SessionId = 1,
                DoctorSectionRoomId = 2,
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
        );
    }
    private static void SeedDoctors(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Doctor>().HasData(
            new
            {
                Id = 1,
                PersonId = 1,                    // علي أحمد
                DepartmentId = 1,                // قسم العلاج الطبيعي
                IsActive = true,
                Specialization = "أخصائي علاج طبيعي",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 2,
                PersonId = 2,                    // محمد صالح
                DepartmentId = 2,                // قسم العظام
                IsActive = true,
                Specialization = "اخصائي اطراف صناعية",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 3,
                PersonId = 3,                    // سارة علي
                DepartmentId = 1,                // قسم الأعصاب
                IsActive = false,                // مثال: في إجازة
                Specialization = "أخصائية أعصاب",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
        );
    }
    private static void SeedDoctorSectionRooms(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DoctorSectionRoom>().HasData(
            new
            {
                Id = 1,
                DoctorId = 1,
                SectionId = 1,
                RoomId = 1,
                AssignDate = SeedDate,
                EndDate = (DateTime?)null,
                IsActive = true,
                Notes = "تكليف أساسي للطبيب في القسم الأول",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 2,
                DoctorId = 2,
                SectionId = 8,
                AssignDate = SeedDate,
                EndDate = (DateTime?)null,
                IsActive = true,
                Notes = "تكليف للطبيب بقسم الحديد",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            },
            new
            {
                Id = 3,
                DoctorId = 3,
                SectionId = 2,
                RoomId = 3,
                AssignDate = SeedDate,
                EndDate = SeedDate,
                IsActive = false,
                Notes = "تكليف منتهي للطبيبة في قسم الحرارة",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
        );
    }
    private static void SeedRepairCards(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RepairCard>().HasData(
            new
            {
                Id = 1,
                DiagnosisId = 1,
                Status = RepairCardStatus.New,
                IsActive = true,
                Notes = "بطاقة صيانة جديدة للحالة الأولى",
                CreatedAtUtc = SeedTimestamp,
                LastModifiedUtc = SeedTimestamp,
                CreatedBy = "Seed",
                LastModifiedBy = "Seed",
                IsDeleted = false
            }
            // ,new
            // {
            //     Id = 2,
            //     DiagnosisId = 2,
            //     Status = RepairCardStatus.InProgress,
            //     IsActive = true,
            //     Notes = "متابعة أعمال الصيانة للحالة الثانية",
            //     CreatedAtUtc = SeedTimestamp,
            //     LastModifiedUtc = SeedTimestamp,
            //     CreatedBy = "Seed",
            //     LastModifiedBy = "Seed",
            //     IsDeleted = false
            // },
            // new
            // {
            //     Id = 3,
            //     DiagnosisId = 3,
            //     Status = RepairCardStatus.Completed,
            //     IsActive = false,
            //     Notes = "تم إكمال أعمال الصيانة وإغلاق البطاقة",
            //     CreatedAtUtc = new DateTimeOffset(2025, 1, 18, 10, 0, 0, TimeSpan.Zero),
            //     LastModifiedUtc = new DateTimeOffset(2025, 1, 18, 10, 0, 0, TimeSpan.Zero),
            //     CreatedBy = "Seed",
            //     LastModifiedBy = "Seed",
            //     IsDeleted = false
            // }
        );
    }

    private static void SeedApplicationPermissions(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationPermission>().HasData(
            // Person (1–4)
            new { Id = 1, Name = Permission.Person.Create, Description = (string?)null },
            new { Id = 2, Name = Permission.Person.Read, Description = (string?)null },
            new { Id = 3, Name = Permission.Person.Update, Description = (string?)null },
            new { Id = 4, Name = Permission.Person.Delete, Description = (string?)null },

            // Service (5–8)
            new { Id = 5, Name = Permission.Service.Create, Description = (string?)null },
            new { Id = 6, Name = Permission.Service.Read, Description = (string?)null },
            new { Id = 7, Name = Permission.Service.Update, Description = (string?)null },
            new { Id = 8, Name = Permission.Service.Delete, Description = (string?)null },

            // Ticket (9–13)
            new { Id = 9, Name = Permission.Ticket.Create, Description = (string?)null },
            new { Id = 10, Name = Permission.Ticket.Read, Description = (string?)null },
            new { Id = 11, Name = Permission.Ticket.Update, Description = (string?)null },
            new { Id = 12, Name = Permission.Ticket.Delete, Description = (string?)null },
            new { Id = 13, Name = Permission.Ticket.Print, Description = (string?)null },

            // Appointment (14–19)
            new { Id = 14, Name = Permission.Appointment.Create, Description = (string?)null },
            new { Id = 15, Name = Permission.Appointment.ReSchedule, Description = (string?)null },
            new { Id = 16, Name = Permission.Appointment.Read, Description = (string?)null },
            new { Id = 17, Name = Permission.Appointment.Update, Description = (string?)null },
            new { Id = 18, Name = Permission.Appointment.Delete, Description = (string?)null },
            new { Id = 19, Name = Permission.Appointment.ChangeStatus, Description = (string?)null },

            // Holiday (20–23)
            new { Id = 20, Name = Permission.Holiday.Create, Description = (string?)null },
            new { Id = 21, Name = Permission.Holiday.Read, Description = (string?)null },
            new { Id = 22, Name = Permission.Holiday.Update, Description = (string?)null },
            new { Id = 23, Name = Permission.Holiday.Delete, Description = (string?)null },

            // TherapyCard (24–30)
            new { Id = 24, Name = Permission.TherapyCard.Create, Description = (string?)null },
            new { Id = 25, Name = Permission.TherapyCard.Read, Description = (string?)null },
            new { Id = 26, Name = Permission.TherapyCard.Update, Description = (string?)null },
            new { Id = 27, Name = Permission.TherapyCard.Delete, Description = (string?)null },
            new { Id = 28, Name = Permission.TherapyCard.Renew, Description = (string?)null },
            new { Id = 29, Name = Permission.TherapyCard.GenerateSessions, Description = (string?)null },
            new { Id = 30, Name = Permission.TherapyCard.CreateSession, Description = (string?)null },

            // RepairCard (31–37)
            new { Id = 31, Name = Permission.RepairCard.Create, Description = (string?)null },
            new { Id = 32, Name = Permission.RepairCard.Read, Description = (string?)null },
            new { Id = 33, Name = Permission.RepairCard.Update, Description = (string?)null },
            new { Id = 34, Name = Permission.RepairCard.Delete, Description = (string?)null },
            new { Id = 35, Name = Permission.RepairCard.ChangeStatus, Description = (string?)null },
            new { Id = 36, Name = Permission.RepairCard.AssignToTechnician, Description = (string?)null },
            new { Id = 37, Name = Permission.RepairCard.CreateDeliveryTime, Description = (string?)null },

            // IndustrialPart (38–41)
            new { Id = 38, Name = Permission.IndustrialPart.Create, Description = (string?)null },
            new { Id = 39, Name = Permission.IndustrialPart.Read, Description = (string?)null },
            new { Id = 40, Name = Permission.IndustrialPart.Update, Description = (string?)null },
            new { Id = 41, Name = Permission.IndustrialPart.Delete, Description = (string?)null },

            // MedicalProgram (42–45)
            new { Id = 42, Name = Permission.MedicalProgram.Create, Description = (string?)null },
            new { Id = 43, Name = Permission.MedicalProgram.Read, Description = (string?)null },
            new { Id = 44, Name = Permission.MedicalProgram.Update, Description = (string?)null },
            new { Id = 45, Name = Permission.MedicalProgram.Delete, Description = (string?)null },

            // Department (46–49)
            new { Id = 46, Name = Permission.Department.Create, Description = (string?)null },
            new { Id = 47, Name = Permission.Department.Read, Description = (string?)null },
            new { Id = 48, Name = Permission.Department.Update, Description = (string?)null },
            new { Id = 49, Name = Permission.Department.Delete, Description = (string?)null },

            // Section (50–53)
            new { Id = 50, Name = Permission.Section.Create, Description = (string?)null },
            new { Id = 51, Name = Permission.Section.Read, Description = (string?)null },
            new { Id = 52, Name = Permission.Section.Update, Description = (string?)null },
            new { Id = 53, Name = Permission.Section.Delete, Description = (string?)null },

            // Room (54–57)
            new { Id = 54, Name = Permission.Room.Create, Description = (string?)null },
            new { Id = 55, Name = Permission.Room.Read, Description = (string?)null },
            new { Id = 56, Name = Permission.Room.Update, Description = (string?)null },
            new { Id = 57, Name = Permission.Room.Delete, Description = (string?)null },

            // Payment (58–61)
            new { Id = 58, Name = Permission.Payment.Create, Description = (string?)null },
            new { Id = 59, Name = Permission.Payment.Read, Description = (string?)null },
            new { Id = 60, Name = Permission.Payment.Update, Description = (string?)null },
            new { Id = 61, Name = Permission.Payment.Delete, Description = (string?)null },

            // Doctor (62–69)
            new { Id = 62, Name = Permission.Doctor.Create, Description = (string?)null },
            new { Id = 63, Name = Permission.Doctor.Read, Description = (string?)null },
            new { Id = 64, Name = Permission.Doctor.Update, Description = (string?)null },
            new { Id = 65, Name = Permission.Doctor.Delete, Description = (string?)null },
            new { Id = 66, Name = Permission.Doctor.AssignDoctorToSection, Description = (string?)null },
            new { Id = 67, Name = Permission.Doctor.AssignDoctorToSectionAndRoom, Description = (string?)null },
            new { Id = 68, Name = Permission.Doctor.ChangeDoctorDepartment, Description = (string?)null },
            new { Id = 69, Name = Permission.Doctor.EndDoctorAssignment, Description = (string?)null },

            // Patient (70–80)
            new { Id = 70, Name = Permission.Patient.Create, Description = (string?)null },
            new { Id = 71, Name = Permission.Patient.Read, Description = (string?)null },
            new { Id = 72, Name = Permission.Patient.Update, Description = (string?)null },
            new { Id = 73, Name = Permission.Patient.Delete, Description = (string?)null },
            new { Id = 74, Name = Permission.Patient.ReadDisabledCard, Description = (string?)null },
            new { Id = 75, Name = Permission.Patient.AddDisabledCard, Description = (string?)null },
            new { Id = 76, Name = Permission.Patient.UpdateDisabledCard, Description = (string?)null },
            new { Id = 77, Name = Permission.Patient.AddWoundedCard, Description = (string?)null },
            new { Id = 78, Name = Permission.Patient.UpdateWoundedCard, Description = (string?)null },
            new { Id = 79, Name = Permission.Patient.ReadWoundedCard, Description = (string?)null },

            // Sale (81–85)
            new { Id = 80, Name = Permission.Sale.Create, Description = (string?)null },
            new { Id = 81, Name = Permission.Sale.Read, Description = (string?)null },
            new { Id = 82, Name = Permission.Sale.Update, Description = (string?)null },
            new { Id = 83, Name = Permission.Sale.Delete, Description = (string?)null },
            new { Id = 84, Name = Permission.Sale.Cancel, Description = (string?)null }
        );
    }


    private static void SeedRoles(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = Role.Admin.ToString(),
                Name = Role.Admin.ToString(),
                NormalizedName = Role.Admin.ToString().ToUpper()
            },
            new IdentityRole
            {
                Id = Role.Receptionist.ToString(),
                Name = Role.Receptionist.ToString(),
                NormalizedName = Role.Receptionist.ToString().ToUpper()
            },
            new IdentityRole
            {
                Id = Role.AccountsEmployee.ToString(),
                Name = Role.AccountsEmployee.ToString(),
                NormalizedName = Role.AccountsEmployee.ToString().ToUpper()
            },
            new IdentityRole
            {
                Id = Role.AccountsManager.ToString(),
                Name = Role.AccountsManager.ToString(),
                NormalizedName = Role.AccountsManager.ToString().ToUpper()
            },
            new IdentityRole
            {
                Id = Role.TechnicalManagementDoctor.ToString(),
                Name = Role.TechnicalManagementDoctor.ToString(),
                NormalizedName = Role.TechnicalManagementDoctor.ToString().ToUpper()
            },
            new IdentityRole
            {
                Id = Role.TechnicalManagementReceptionist.ToString(),
                Name = Role.TechnicalManagementReceptionist.ToString(),
                NormalizedName = Role.TechnicalManagementReceptionist.ToString().ToUpper()
            },
            new IdentityRole
            {
                Id = Role.TechnicalManagementOrdersEmployee.ToString(),
                Name = Role.TechnicalManagementOrdersEmployee.ToString(),
                NormalizedName = Role.TechnicalManagementOrdersEmployee.ToString().ToUpper()
            },
            new IdentityRole
            {
                Id = Role.TechnicalManagementManager.ToString(),
                Name = Role.TechnicalManagementManager.ToString(),
                NormalizedName = Role.TechnicalManagementManager.ToString().ToUpper()
            },
            new IdentityRole
            {
                Id = Role.TherapyManagementReceptionist.ToString(),
                Name = Role.TherapyManagementReceptionist.ToString(),
                NormalizedName = Role.TherapyManagementReceptionist.ToString().ToUpper()
            },
            new IdentityRole
            {
                Id = Role.TherapyManagementDoctor.ToString(),
                Name = Role.TherapyManagementDoctor.ToString(),
                NormalizedName = Role.TherapyManagementDoctor.ToString().ToUpper()
            },
            new IdentityRole
            {
                Id = Role.TherapyManagementManager.ToString(),
                Name = Role.TherapyManagementManager.ToString(),
                NormalizedName = Role.TherapyManagementManager.ToString().ToUpper()
            },
            new IdentityRole
            {
                Id = Role.SalesEmployee.ToString(),
                Name = Role.SalesEmployee.ToString(),
                NormalizedName = Role.SalesEmployee.ToString().ToUpper()
            },
            new IdentityRole
            {
                Id = Role.SalesManager.ToString(),
                Name = Role.SalesManager.ToString(),
                NormalizedName = Role.SalesManager.ToString().ToUpper()
            },
            new IdentityRole
            {
                Id = Role.ExchangeOrderEmployee.ToString(),
                Name = Role.ExchangeOrderEmployee.ToString(),
                NormalizedName = Role.ExchangeOrderEmployee.ToString().ToUpper()
            },
            new IdentityRole
            {
                Id = Role.ExchangeOrderManager.ToString(),
                Name = Role.ExchangeOrderManager.ToString(),
                NormalizedName = Role.ExchangeOrderManager.ToString().ToUpper()
            },
            new IdentityRole
            {
                Id = Role.PurchaseEmployee.ToString(),
                Name = Role.PurchaseEmployee.ToString(),
                NormalizedName = Role.PurchaseEmployee.ToString().ToUpper()
            },
            new IdentityRole
            {
                Id = Role.PurchaseManager.ToString(),
                Name = Role.PurchaseManager.ToString(),
                NormalizedName = Role.PurchaseManager.ToString().ToUpper()
            }
        );
    }
    private static void SeedAdminRolePermissions(ModelBuilder modelBuilder)
    {
        var rolePermissions = new List<RolePermission>();

        // Admin gets ALL permissions (IDs 1 through 84)
        for (int permissionId = 1; permissionId <= 84; permissionId++)
        {
            rolePermissions.Add(new RolePermission
            {
                RoleId = Role.Admin.ToString(),  // "Admin"
                PermissionId = permissionId
            });
        }

        modelBuilder.Entity<RolePermission>().HasData(rolePermissions.ToArray());
    }
    private static void SeedAdminUser(ModelBuilder modelBuilder)
    {
        var adminId = "19a59129-6c20-417a-834d-11a208d32d96";

        modelBuilder.Entity<AppUser>().HasData(
            new AppUser
            {
                Id = adminId,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                EmailConfirmed = true,

                // pre-generated password hash - Admin@12345
                PasswordHash = "AQAAAAIAAYagAAAAEJ8FZJNBD5y7YVavcn6e99DgR9n2YPF5mD31ySEh7F3hW6Y2qgFlgVzuqMbI8mgZZg==",

                SecurityStamp = "f81ff42e-eb5b-4af3-a3c8-4ff90f17fe1d",
                ConcurrencyStamp = "45a69252-a993-46d4-aa95-6dc881c3a15a",

                PhoneNumber = null,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,

                PersonId = 4,  // must exist in People table
                IsActive = true
            }
        );
    }
}