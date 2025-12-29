namespace AlatrafClinic.Application.Features.Doctors.Dtos;

public sealed class TherapistSessionProgramDto
{
    public int SessionProgramId { get; set; }
    public int DiagnosisProgramId { get; set; }
    public string? ProgramName { get; set; }
    public int? SessionId { get; set; }
    public int SessionNumber { get; set; }
    public DateOnly SessionDate { get; set; }
    public int TherapyCardId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string PatientPhoneNumber { get; set; } = string.Empty;
}
