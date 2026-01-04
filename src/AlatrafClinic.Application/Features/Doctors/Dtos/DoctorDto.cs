using AlatrafClinic.Application.Features.People.Dtos;

namespace AlatrafClinic.Application.Features.Doctors.Dtos;

public class DoctorDto
{
    public int DoctorId { get; set; }
    public PersonDto? PersonDto { get; set; }
    public string? Specialization { get; set; }
    public int DepartmentId { get; set; }
    public int? SectionId { get; set; }
    public int? RoomId { get; set; }
    public bool IsActive { get; set; }
    public bool HasAssignments { get; set; }
}