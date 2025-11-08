using AlatrafClinic.Domain.Diagnosises.Enums;

namespace AlatrafClinic.Application.Features.Diagnosises.Dtos;

public class DiagnosisListItemDto
{
    public int Id { get; set; }
    public string DiagnosisText { get; set; } = string.Empty;

    public DateTime InjuryDate { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int TicketNumber { get; set; }
    public DiagnosisType Type { get; set; }

    public List<string> Programs { get; set; } = new();
    public List<string> IndustrialParts { get; set; } = new();

    public bool HasRepairCard { get; set; }
    public bool HasSale { get; set; }
    public bool HasTherapyCards { get; set; }
}