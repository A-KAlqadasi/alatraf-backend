using AlatrafClinic.Application.Features.Organization.Rooms.Dtos;
using AlatrafClinic.Application.Features.Organization.Sections.Dtos;

namespace AlatrafClinic.Application.Features.People.Doctors.Dtos;

public class DoctorSectionRoomDto
{
    public int DoctorSectionRoomId { get; set; }
    public DoctorDto? Doctor { get; set; }
    public SectionDto? Section { get; set; }
    public RoomDto? Room { get; set; }
    public DateTime AssignDate { get; set; }
    public DateTime? EndAssignDate { get; set; }
    public bool IsActive { get; set; }
    public int? NumberOfIndustrialParts { get; set; }
    public int? NumberOfSessions { get; set; }
}