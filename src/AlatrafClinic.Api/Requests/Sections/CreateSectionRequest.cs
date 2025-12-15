using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.Sections;

public sealed class CreateSectionRequest
{
    [Range(1,int.MaxValue, ErrorMessage = "Department Id must be greater than 0")]
    [Required(ErrorMessage = "Department Id is required")]
    public int DepartmentId { get; set; }
    [Required(ErrorMessage = "Department Id is required")]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}
