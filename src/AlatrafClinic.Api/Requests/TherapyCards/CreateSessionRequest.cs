using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.TherapyCards;

public sealed class CreateSessionRequest
{
    [Required( ErrorMessage = "TherapyCardId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "TherapyCardId must be greater than 0.")]
    public List<SessionProgramRequest> SessionPrograms { get; set; } = new();
}

public sealed class SessionProgramRequest
{
    [Required( ErrorMessage = "DiagnosisProgramId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "DiagnosisProgramId must be greater than 0.")]
    public int DiagnosisProgramId { get; set; }

    [Required( ErrorMessage = "DoctorId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "DoctorId must be greater than 0.")]
    public int DoctorId { get; set; }
    
    [Required( ErrorMessage = "SectionId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "SectionId must be greater than 0.")]
    public int SectionId { get; set; }

    [Required( ErrorMessage = "RoomId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "RoomId must be greater than 0.")]
    public int RoomId { get; set; }
}