using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.RepairCards.Enums;

namespace AlatrafClinic.Application.Features.RepairCards.Dtos;

public class RepairCardDto
{
    public int RepairCardId { get; set; }
    public DiagnosisDto Diagnosis { get; set; } = new();
    public bool IsActive { get; set; }
    public bool IsLate { get; set; }
    public string CardStatus { get; set; } = string.Empty;
    public List<DiagnosisIndustrialPartDto>? DiagnosisIndustrialParts { get; set; }
    public DateTime DeliveryDate { get; set; }
    public decimal TotalCost { get; set; }
}