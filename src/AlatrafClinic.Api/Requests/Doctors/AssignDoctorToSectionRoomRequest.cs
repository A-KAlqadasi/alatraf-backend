using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.Doctors;

public class AssignDoctorToSectionRoomRequest
{
    [Required]
    public int SectionId { get; set; }
    public int? RoomId { get; set; }
    [Required]
    public bool IsActive { get; set; }
    [MaxLength(500)]
    public string? Notes { get; set; }
}