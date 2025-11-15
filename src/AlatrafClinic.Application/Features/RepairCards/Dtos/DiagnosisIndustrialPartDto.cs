namespace AlatrafClinic.Application.Features.RepairCards.Dtos;

public class DiagnosisIndustrialPartDto
{
    public int DiagnosisIndustrialPartId { get; set; }
    public int IndustrialPartId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public int? DoctorSectionRoomId { get; set; }
    public string? DoctorSectionName { get; set; }
    public DateTime? DoctorAssignedDate { get; set; }
    public decimal TotalPrice { get; set; }
}