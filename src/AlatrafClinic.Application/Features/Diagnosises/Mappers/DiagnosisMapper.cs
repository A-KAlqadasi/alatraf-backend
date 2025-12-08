using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Patients.Mappers;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Application.Features.Sales.Dtos;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.DiagnosisIndustrialParts;
using AlatrafClinic.Domain.Diagnosises.DiagnosisPrograms;
using AlatrafClinic.Domain.Diagnosises.Enums;
using AlatrafClinic.Domain.Diagnosises.InjuryReasons;
using AlatrafClinic.Domain.Diagnosises.InjurySides;
using AlatrafClinic.Domain.Diagnosises.InjuryTypes;
using AlatrafClinic.Domain.Sales.SalesItems;

namespace AlatrafClinic.Application.Features.Diagnosises.Mappers;

public static class DiagnosisMapper
{
    public static DiagnosisDto ToDto(this Diagnosis diagnosis)
    {
        // Build optional related collections only if present (avoid empty arrays in payloads if you prefer)
        var programs = diagnosis.DiagnosisPrograms?.Any() == true
            ? diagnosis.DiagnosisPrograms!.ToDtos()
            : null;

        var industrialParts = diagnosis.DiagnosisIndustrialParts?.Any() == true
            ? diagnosis.DiagnosisIndustrialParts!.ToDtos()
            : null;

        var saleItems = diagnosis.Sale?.SaleItems?.Any() == true
            ? diagnosis.Sale!.SaleItems!.ToDtos()
            : (List<SaleItemDto>?)null;

        return new DiagnosisDto
        {
            DiagnosisId   = diagnosis.Id,
            DiagnosisText = diagnosis.DiagnosisText,
            InjuryDate    = diagnosis.InjuryDate,

            TicketId      = diagnosis.TicketId,
            PatientId     = diagnosis.PatientId,

            // Prefer PatientName from Patient.Person if available; fallback to Ticket.Patient.Person
            PatientName   = diagnosis.Patient?.Person?.FullName
                            ?? diagnosis.Ticket?.Patient?.Person?.FullName
                            ?? string.Empty,

            // Full Patient DTO if your Patient mapper exists
            Patient       = diagnosis.Patient?.ToDto()
                            ?? diagnosis.Ticket?.Patient?.ToDto(),

            DiagnosisType = diagnosis.DiagnoType.ToArabicDiagnosisType(),

            InjuryReasons = diagnosis.InjuryReasons.ToDtos(),
            InjurySides   = diagnosis.InjurySides.ToDtos(),
            InjuryTypes   = diagnosis.InjuryTypes.ToDtos(),

            // New optional related collections
            Programs        = programs,
            IndustrialParts = industrialParts,
            SaleItems       = saleItems,

            // Relationship flags
            HasTherapyCards = diagnosis.TherapyCard != null,
            HasRepairCard   = diagnosis.RepairCard != null,
            HasSale         = diagnosis.Sale != null
        };
    }

    public static List<DiagnosisDto> ToDtos(this IEnumerable<Diagnosis> diagnosises)
        => diagnosises.Select(d => d.ToDto()).ToList();

    public static InjuryDto ToDto(this InjuryReason reason)
        => new()
        {
            Id   = reason.Id,
            Name = reason.Name
        };

    public static InjuryDto ToDto(this InjurySide side)
        => new()
        {
            Id   = side.Id,
            Name = side.Name
        };

    public static InjuryDto ToDto(this InjuryType type)
        => new()
        {
            Id   = type.Id,
            Name = type.Name
        };

    public static List<InjuryDto> ToDtos(this IEnumerable<InjuryReason> reasons)
        => reasons.Select(r => r.ToDto()).ToList();

    public static List<InjuryDto> ToDtos(this IEnumerable<InjurySide> sides)
        => sides.Select(s => s.ToDto()).ToList();

    public static List<InjuryDto> ToDtos(this IEnumerable<InjuryType> types)
        => types.Select(t => t.ToDto()).ToList();

    public static List<DiagnosisProgramDto> ToDtos(this IEnumerable<DiagnosisProgram> programs)
        => programs.Select(p => new DiagnosisProgramDto
        {
            DiagnosisProgramId = p.Id,
            ProgramName        = p.MedicalProgram?.Name ?? string.Empty,
            MedicalProgramId   = p.MedicalProgramId,
            Duration           = p.Duration,
            Notes              = p.Notes
        }).ToList();

    public static List<DiagnosisIndustrialPartDto> ToDtos(this IEnumerable<DiagnosisIndustrialPart> parts)
        => parts.Select(part => new DiagnosisIndustrialPartDto
        {
            DiagnosisIndustrialPartId = part.Id,
            IndustrialPartId = part.IndustrialPartUnit?.IndustrialPartId ?? 0,
            PartName = part.IndustrialPartUnit?.IndustrialPart?.Name ?? string.Empty,
            UnitId = part.IndustrialPartUnit?.UnitId ?? 0,
            UnitName = part.IndustrialPartUnit?.Unit?.Name ?? string.Empty,
            Quantity = part.Quantity,
            Price = part.Price,
            DoctorSectionRoomId = part.DoctorSectionRoomId,
            DoctorSectionName = part.DoctorSectionRoom?.Section?.Name,
            DoctorAssignedDate = part.DoctorSectionRoom?.AssignDate,
            TotalPrice = part.Quantity * part.Price
        }).ToList();

    public static List<SaleItemDto> ToDtos(this IEnumerable<SaleItem> saleItems)
        => saleItems.Select(item => new SaleItemDto
        {
            SaleItemId = item.Id,
            UnitId     = item.ItemUnit?.UnitId ?? 0,
            UnitName   = item.ItemUnit?.Unit?.Name ?? string.Empty,
            ItemId     = item.ItemUnit?.ItemId ?? 0,
            ItemName   = item.ItemUnit?.Item?.Name ?? string.Empty,
            Quantity   = item.Quantity,
            Price      = item.Price,
        }).ToList();

    public static string ToArabicDiagnosisType(this DiagnosisType type)
    {
        switch (type)
        {
            case DiagnosisType.Therapy : return "علاج طبيعي";
            case DiagnosisType.Limbs : return "أطراف صناعية";
            case DiagnosisType.Sales : return "مبيعات";
            
            default:
             throw new Exception($"Diagnosis type {type.ToString()} is now unknown");
        }
    }
}