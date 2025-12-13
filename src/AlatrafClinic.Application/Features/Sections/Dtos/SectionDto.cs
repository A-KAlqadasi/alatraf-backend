using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Application.Features.Rooms.Dtos;

namespace AlatrafClinic.Application.Features.Sections.Dtos;
public class SectionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int? RoomsCount { get; set; }
}