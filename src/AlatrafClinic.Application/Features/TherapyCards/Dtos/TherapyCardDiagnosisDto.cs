using AlatrafClinic.Application.Features.Diagnosises.Dtos;

namespace AlatrafClinic.Application.Features.TherapyCards.Dtos;

public class TherapyCardDiagnosisDto
{
    public int TicketId { get; set; }
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string Gender {get; set; } = string.Empty;
    public int Age { get; set; }

    public int DiagnosisId { get; set; }
    public string DiagnosisText { get; set; } = string.Empty;
    public DateOnly InjuryDate { get; set; }
    public string DiagnosisType { get; set; } = string.Empty;

    public List<InjuryDto> InjuryReasons { get; set; } = new();
    public List<InjuryDto> InjurySides { get; set; } = new();
    public List<InjuryDto> InjuryTypes { get; set; } = new();

    public List<DiagnosisProgramDto>? Programs { get; set; }
    
    public int TherapyCardId { get; set; }
    public DateOnly? ProgramStartDate { get; set; }
    public DateOnly? ProgramEndDate { get; set; }
    public string TherapyCardType { get; set; } = string.Empty;
    public string CardStatus { get; set; } = string.Empty;
    public string? Notes { get; set; }
}