using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.MedicalPrograms;

public class UpdateMedicalProgramRequest
{
    [Required(ErrorMessage = "Medical Program Name is required.")]
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int? SectionId { get; set; }
}