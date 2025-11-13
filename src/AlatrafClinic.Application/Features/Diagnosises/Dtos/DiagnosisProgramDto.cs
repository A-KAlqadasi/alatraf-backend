namespace AlatrafClinic.Application.Features.Diagnosises.Dtos;

public class DiagnosisProgramDto
{
    public int DiagnosisProgramId { get; set; }
    public int MedicalProgramId { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string? Notes { get; set; }
}