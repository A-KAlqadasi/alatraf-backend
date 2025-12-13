using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.Doctors;

public class AssignDoctorToSectionRequest
{
    [Required]
    public int SectionId { get; set; }
    [MaxLength(100)]
    public string? Notes { get; set; }
}
