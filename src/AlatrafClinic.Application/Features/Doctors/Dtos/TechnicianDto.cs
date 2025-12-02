namespace AlatrafClinic.Application.Features.Doctors.Dtos;

public class TechnicianDto
{
    public int DoctorSectionRoomId { get; set; }
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public int SectionId { get; set; }
    public string SectionName { get; set; } = string.Empty;
    public int TodayIndustrialParts { get; set; }
}