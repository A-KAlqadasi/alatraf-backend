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
    public DiagnosisType DiagnosisType { get; set; }
    public List<InjuryDto> InjuryReasons { get; set; } = new();
    public List<InjuryDto> InjurySides { get; set; } = new();
    public List<InjuryDto> InjuryTypes { get; set; } = new();
}