using AlatrafClinic.Application.Features.People.Doctors.Dtos;
using AlatrafClinic.Application.Features.Rooms.Dtos;

namespace AlatrafClinic.Application.Features.Sections.Dtos;
public class SectionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public List<RoomDto>? Rooms { get; set; }
    public List<DoctorDto> Doctors { get; set; } = new();
}