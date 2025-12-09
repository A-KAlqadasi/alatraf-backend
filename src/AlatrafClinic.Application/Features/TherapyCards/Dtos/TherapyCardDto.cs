using AlatrafClinic.Application.Features.Diagnosises.Dtos;

namespace AlatrafClinic.Application.Features.TherapyCards.Dtos;

public class TherapyCardDto
{
    public int TherapyCardId { get; set; }
    public DiagnosisDto Diagnosis { get; set; } = new();
    public bool IsActive { get; set; }
    public int? NumberOfSessions { get; set; }
    public DateTime? ProgramStartDate { get; set; }
    public DateTime? ProgramEndDate { get; set; }
    public string TherapyCardType { get; set; } = string.Empty;
    public string CardStatus { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public List<DiagnosisProgramDto>? Programs { get; set; }
    public List<SessionDto>? Sessions { get; set; }
}
