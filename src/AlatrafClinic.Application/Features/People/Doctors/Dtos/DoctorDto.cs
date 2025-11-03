

using AlatrafClinic.Application.Features.People.Persons.Dtos;

namespace AlatrafClinic.Application.Features.People.Doctors.Dtos
{
    public class DoctorDto
    {
        public int DoctorId { get; set; }
        public int PersonId { get; set; }
        public PersonDto? PersonDto { get; set; }
        public string? Specialization { get; set; }
        public int DepartmentId { get; set; }
    }
}