namespace AlatrafClinic.Application.Features.TherapyCards.Dtos;

public class SessionDto
{
    public int SessionId { get; set; }
    public bool IsTaken { get; set; }
    public int Number { get; set; }
    public DateTime SessionDate { get; set; }

    public List<SessionProgramDto> SessionPrograms { get; set; } = new();
}
