namespace AlatrafClinic.Application.Features.TherapyCards.Dtos;

public class SessionListDto
{
    public int SessionId { get; set; }
    public bool IsTaken { get; set; }
    public int Number { get; set; }
    public DateTime SessionDate { get; set; }

    public int TherapyCardId { get; set; }
    public string? TherapyCardType { get; set; }
    public DateTime ProgramStartDate { get; set; }
    public DateTime ProgramEndDate { get; set; }

    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;

    public List<SessionProgramDto> SessionPrograms { get; set; } = new();
}