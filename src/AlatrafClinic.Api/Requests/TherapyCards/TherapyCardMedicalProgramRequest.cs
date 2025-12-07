using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.TherapyCards;

public sealed class TherapyCardMedicalProgramRequest
{
    [Required(ErrorMessage = "MedicalProgramId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "MedicalProgramId must be greater than 0.")]
    public int MedicalProgramId { get; set; }

    [Required(ErrorMessage = "Duration is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Duration must be greater than 0.")]
    public int Duration { get; set; }
    public string? Notes { get; set; }
}
