using AlatrafClinic.Application.Features.People.Dtos;

namespace AlatrafClinic.Application.Features.Doctors.Dtos;

public class DoctorDto
{
    public int DoctorId { get; set; }
    public PersonDto? PersonDto { get; set; }
    public string? Specialization { get; set; }
    public int DepartmentId { get; set; }
}
