using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.People.Patients.Mappers;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.DiagnosisIndustrialParts;
using AlatrafClinic.Domain.Diagnosises.DiagnosisPrograms;
using AlatrafClinic.Domain.Diagnosises.InjuryReasons;
using AlatrafClinic.Domain.Diagnosises.InjurySides;
using AlatrafClinic.Domain.Diagnosises.InjuryTypes;
using AlatrafClinic.Domain.Sales.SalesItems;

namespace AlatrafClinic.Application.Features.Diagnosises.Mappers;

public static class DiagnosisMapper
{
    public static DiagnosisDto ToDto(this Diagnosis diagnosis)
    {
        return new DiagnosisDto
        {
            DiagnosisId = diagnosis.Id,
            DiagnosisText = diagnosis.DiagnosisText,
            InjuryDate = diagnosis.InjuryDate,
            TicketId = diagnosis.TicketId,
            PatientId = diagnosis.PatientId,
            PatientName = diagnosis.Ticket?.Patient?.Person?.FullName ?? string.Empty,
            DiagnosisType = diagnosis.DiagnoType,
            Patient = diagnosis.Ticket?.Patient?.ToDto(),
            InjuryReasons = diagnosis.InjuryReasons.ToDtos(),
            InjurySides = diagnosis.InjurySides.ToDtos(),
            InjuryTypes = diagnosis.InjuryTypes.ToDtos(),
        };
    }

    public static List<DiagnosisDto> ToDtos(this IEnumerable<Diagnosis> diagnosises)
    {
        return diagnosises.Select(d => d.ToDto()).ToList();
    }

    public static InjuryDto ToDto(this InjuryReason reason)
    {
        return new InjuryDto
        {
            Id = reason.Id,
            Name = reason.Name
        };
    }
    public static InjuryDto ToDto(this InjurySide side)
    {
        return new InjuryDto
        {
            Id = side.Id,
            Name = side.Name
        };
    }
    public static InjuryDto ToDto(this InjuryType type)
    {
        return new InjuryDto
        {
            Id = type.Id,
            Name = type.Name
        };
    }
    public static List<InjuryDto> ToDtos(this IEnumerable<InjuryReason> reasons)
    {
        return reasons.Select(r => r.ToDto()).ToList();
    }
    public static List<InjuryDto> ToDtos(this IEnumerable<InjurySide> sides)
    {
        return sides.Select(s => s.ToDto()).ToList();
    }
    public static List<InjuryDto> ToDtos(this IEnumerable<InjuryType> types)
    {
        return types.Select(t => t.ToDto()).ToList();
    }
    public static List<DiagnosisProgramDto> ToDtos(this IEnumerable<DiagnosisProgram> programs)
    {
        return programs.Select(p => new DiagnosisProgramDto
        {
            DiagnosisProgramId = p.Id,
            ProgramName = p.MedicalProgram?.Name ?? string.Empty,
            MedicalProgramId = p.MedicalProgramId,
            Duration = p.Duration,
            Notes = p.Notes
        }).ToList();
    }
    public static List<DiagnosisIndustrialPartDto> ToDtos(this IEnumerable<DiagnosisIndustrialPart> parts)
    {
        return parts.Select(part => new DiagnosisIndustrialPartDto
        {
            DiagnosisIndustrialPartId = part.Id,
            IndustrialPartId = part.IndustrialPartUnit?.IndustrialPartId ?? 0,
            PartName = part.IndustrialPartUnit?.IndustrialPart?.Name ?? string.Empty,
            UnitId = part.IndustrialPartUnit?.UnitId ?? 0,
            UnitName = part.IndustrialPartUnit?.Unit?.Name ?? string.Empty,
            Quantity = part.Quantity,
            Price = part.Price
        }).ToList();
    }
    public static List<SaleItemDto> ToDtos(this IEnumerable<SaleItem> saleItems)
    {
        return saleItems.Select(item => new SaleItemDto
        {
            SaleItemId = item.Id,
            UnitId = item.ItemUnit?.UnitId ?? 0,
            UnitName = item.ItemUnit?.Unit?.Name ?? string.Empty,
            ItemId = item.ItemUnit?.ItemId ?? 0,
            ItemName = item.ItemUnit?.Item?.Name ?? string.Empty,
            Quantity = item.Quantity,
            Price = item.Price,
        }).ToList();
    }
}