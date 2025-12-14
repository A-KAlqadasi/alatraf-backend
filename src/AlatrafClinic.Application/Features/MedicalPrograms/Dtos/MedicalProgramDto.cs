namespace AlatrafClinic.Application.Features.MedicalPrograms.Dtos;

public class MedicalProgramDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int? SectionId { get; set; }
    public string? SectionName { get; set; }
}