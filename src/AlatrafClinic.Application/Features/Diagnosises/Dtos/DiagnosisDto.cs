using System.Security.Cryptography.X509Certificates;

using AlatrafClinic.Domain.Diagnosises.Enums;

namespace AlatrafClinic.Application.Features.Diagnosises.Dtos;

public class DiagnosisDto
{
    public int DiagnosisId { get; set; }
    public string DiagnosisText { get; set; } = string.Empty;
    public DateTime InjuryDate { get; set; }
    public int TicketId { get; set; }
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public DiagnosisType DiagnosisType { get; set; }
    public List<InjuryDto> InjuryReasons { get; set; } = new();
    public List<InjuryDto> InjurySides { get; set; } = new();
    public List<InjuryDto> InjuryTypes { get; set; } = new();
    public List<DiagnosisProgramDto>? Programs { get; set; }
    public List<DiagnosisIndustrialPartDto>? IndustrialParts { get; set; }
    public List<SaleItemDto>? SaleItems { get; set; }
    
}