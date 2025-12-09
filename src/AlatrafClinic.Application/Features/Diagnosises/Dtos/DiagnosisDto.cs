using AlatrafClinic.Application.Features.Patients.Dtos;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Application.Features.Sales.Dtos;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;

namespace AlatrafClinic.Application.Features.Diagnosises.Dtos;

public class DiagnosisDto
{
    public int DiagnosisId { get; set; }
    public string DiagnosisText { get; set; } = string.Empty;
    public DateTime InjuryDate { get; set; }

    public int TicketId { get; set; }
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public PatientDto? Patient { get; set; }

    public string DiagnosisType { get; set; } = string.Empty;

    public List<InjuryDto> InjuryReasons { get; set; } = new();
    public List<InjuryDto> InjurySides { get; set; } = new();
    public List<InjuryDto> InjuryTypes { get; set; } = new();

    // 👇 New: optional related details
    public List<DiagnosisProgramDto>? Programs { get; set; }
    public List<DiagnosisIndustrialPartDto>? IndustrialParts { get; set; }
    public List<SaleItemDto>? SaleItems { get; set; }

    // 👇 New: quick relationship flags
    public bool HasTherapyCards { get; set; }
    public bool HasRepairCard { get; set; }
    public bool HasSale { get; set; }
}