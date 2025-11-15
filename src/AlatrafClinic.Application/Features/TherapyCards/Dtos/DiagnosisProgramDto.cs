namespace AlatrafClinic.Application.Features.TherapyCards.Dtos;

public class DiagnosisProgramDto
{
    public int DiagnosisProgramId { get; set; }
    public int MedicalProgramId { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string? Notes { get; set; }
}