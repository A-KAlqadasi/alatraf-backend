using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.TherapyCards;

public sealed class TakeSessionRequest
{
    [Required(ErrorMessage = "DiagnosisProgramId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Diagnosis Program Id must be greater than 0.")]
    public int DiagnosisProgramId {get; set; }

    [Required(ErrorMessage = "DoctorSectionRoomId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Doctor Section Room Id must be greater than 0.")]
    public int DoctorSectionRoomId {get; set; }

}