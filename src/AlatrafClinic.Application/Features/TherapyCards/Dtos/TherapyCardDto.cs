using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.TherapyCards;
using AlatrafClinic.Domain.TherapyCards.Enums;

namespace AlatrafClinic.Application.Features.TherapyCards.Dtos;

public class TherapyCardDto
{
    public int TherapyCardId { get; set; }
    public DiagnosisDto Diagnosis { get; set; } = new();
    public bool IsActive { get; set; }
    public int? NumberOfSessions { get; set; }
    public DateTime? ProgramStartDate { get; set; }
    public DateTime? ProgramEndDate { get; set; }
    public TherapyCardType TherapyCardType { get; set; }
    public CardStatus CardStatus { get; set; }
    public string? Notes { get; set; }
    public List<DiagnosisProgramDto>? Programs { get; set; }
}