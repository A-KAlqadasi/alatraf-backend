using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.Sections;

public sealed class UpdateSectionRequest
{
    [Required(ErrorMessage = "Department Id is required")]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}