namespace AlatrafClinic.Application.Features.Doctors.Dtos;

public class TherapistDto
{
    public int? DoctorSectionRoomId { get; set; }
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public int? SectionId { get; set; }
    public string? SectionName { get; set; } = string.Empty;
    public int? RoomId { get; set; }
    public string? RoomName { get; set; } = string.Empty;
    public int TodaySessions { get; set; }
}

public sealed class TherapistSessionProgramDto
{
    public int SessionProgramId { get; set; }
    public int DiagnosisProgramId { get; set; }
    public string? ProgramName { get; set; }
    public int? SessionId { get; set; }
    public int TherapyCardId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string PatientPhoneNumber { get; set; } = string.Empty;
}


public sealed class TherapistTodaySessionsResultDto
{
    public int DoctorSectionRoomId { get; set; }

    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;

    public int SectionId { get; set; }
    public string SectionName { get; set; } = string.Empty;

    public int RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;

    public DateOnly Date { get; set; }

    public List<TherapistSessionProgramDto> Items { get; set; } = new();
}