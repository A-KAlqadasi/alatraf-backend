using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.RepairCards;

public sealed class DoctorAssignmentRequest
{
    [Required(ErrorMessage = "DoctorId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "DoctorId must be a positive integer.")]
    public int DoctorId { get; set; }
    
    [Required(ErrorMessage = "SectionId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "SectionId must be a positive integer.")]
    public int SectionId { get; set; }
}