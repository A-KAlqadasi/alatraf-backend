namespace AlatrafClinic.Application.Features.Doctors.Dtos;

public class TechnicianIndustrialPartsResultDto
{
    public int DoctorSectionRoomId { get; set; }

    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;

    public int SectionId { get; set; }
    public string SectionName { get; set; } = string.Empty;

    public DateOnly Date { get; set; }

    public List<TechnicianIndustrialPartDto> Items { get; set; } = new();
}

public class TechnicianIndustrialPartDto
{
    public int DiagnosisIndustrialPartId { get; set; }
    public int IndustrialPartUnitId { get; set; }
    public int Quantity { get; set; }
    public string? IndustrialPartName { get; set; }
    public int RepairCardId { get; set; }
    public string? PatientName { get; set; }
    public string? PatientPhoneNumber { get; set; }
}

