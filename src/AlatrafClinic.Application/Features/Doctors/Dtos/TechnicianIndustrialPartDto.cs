namespace AlatrafClinic.Application.Features.Doctors.Dtos;

public class TechnicianIndustrialPartDto
{
    public int DiagnosisIndustrialPartId { get; set; }
    public int IndustrialPartUnitId { get; set; }
    public int Quantity { get; set; }
    public string? IndustrialPartName { get; set; }
    public string? UnitName { get; set; } 
    public int RepairCardId { get; set; }
    public string? PatientName { get; set; }
    public string? PatientPhoneNumber { get; set; }
}

