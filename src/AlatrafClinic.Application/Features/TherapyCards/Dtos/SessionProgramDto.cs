namespace AlatrafClinic.Application.Features.TherapyCards.Dtos;

public class SessionProgramDto
{
    public int SessionProgramId { get; set; }
    public int DiagnosisProgramId { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public int DoctorSectionRoomId { get; set; }
    public string? DoctorSectionRoomName { get; set; }
    public string? DoctorName { get; set; }
}