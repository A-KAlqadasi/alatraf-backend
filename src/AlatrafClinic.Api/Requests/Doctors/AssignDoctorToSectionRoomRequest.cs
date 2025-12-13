using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.Doctors;

public class AssignDoctorToSectionRoomRequest
{
    [Required]
    public int SectionId { get; set; }
    [Required]
    public int RoomId { get; set; }
    [MaxLength(100)]
    public string? Notes { get; set; }
}