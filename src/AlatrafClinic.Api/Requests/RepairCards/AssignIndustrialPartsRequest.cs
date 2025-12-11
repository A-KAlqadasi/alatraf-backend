using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.RepairCards;

public sealed class IndustrialPartAssignmentItem
{
    [Required(ErrorMessage = "DiagnosisIndustrialPartId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "DiagnosisIndustrialPartId must be a positive integer.")]
    public int DiagnosisIndustrialPartId { get; set; }

    [Required(ErrorMessage = "DoctorId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "DoctorId must be a positive integer.")]
    public int DoctorId { get; set; }
    
    [Required(ErrorMessage = "SectionId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "SectionId must be a positive integer.")]
    public int SectionId { get; set; }
}

public sealed class AssignIndustrialPartsRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "At least one industrial part assignemnt is required.")]    
    public List<IndustrialPartAssignmentItem> Assignments { get; set; } = new();
}