using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.InjuryReasons;
using AlatrafClinic.Domain.Diagnosises.InjurySides;
using AlatrafClinic.Domain.Diagnosises.InjuryTypes;

namespace AlatrafClinic.Application.Features.Diagnosises.Mappers;

public static class DiagnosisMapper
{
    public static DiagnosisDto ToDto(this Diagnosis diagnosis)
    {
        return new DiagnosisDto
        {
            Id = diagnosis.Id,
            DiagnosisText = diagnosis.DiagnosisText,
            InjuryDate = diagnosis.InjuryDate,
            TicketId = diagnosis.TicketId,
            PatientId = diagnosis.PatientId,
            DiagnosisType = diagnosis.DiagnoType,
            InjuryReasons = diagnosis.InjuryReasons.Select(r => r.ToDto()).ToList(),
            InjurySides = diagnosis.InjurySides.Select(s => s.ToDto()).ToList(),
            InjuryTypes = diagnosis.InjuryTypes.Select(t => t.ToDto()).ToList()
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
}