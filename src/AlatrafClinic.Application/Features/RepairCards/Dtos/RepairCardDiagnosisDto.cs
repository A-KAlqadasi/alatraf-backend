using System.Security.Cryptography.X509Certificates;

using AlatrafClinic.Application.Features.Diagnosises.Dtos;

namespace AlatrafClinic.Application.Features.RepairCards.Dtos;

public class RepairCardDiagnosisDto
{
    public int RepairCardId { get; set; }
    public int TicketId { get; set; }
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string Gender {get; set; } = string.Empty;
    public int Age { get; set; }
    public bool IsActive { get; set; }

    public int DiagnosisId { get; set; }
    public string DiagnosisText { get; set; } = string.Empty;
    public DateOnly InjuryDate { get; set; }
    public string DiagnosisType { get; set; } = string.Empty;
    public string CardStatus { get; set; } = string.Empty;

    public List<InjuryDto> InjuryReasons { get; set; } = new();
    public List<InjuryDto> InjurySides { get; set; } = new();
    public List<InjuryDto> InjuryTypes { get; set; } = new();
    public decimal TotalCost { get; set; }

    public List<DiagnosisIndustrialPartDto>? DiagnosisIndustrialParts { get; set; }
    
}