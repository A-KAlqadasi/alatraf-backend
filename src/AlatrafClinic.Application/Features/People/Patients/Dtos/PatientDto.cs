using AlatrafClinic.Application.Features.People.Persons.Dtos;
using AlatrafClinic.Domain.Patients.Enums;

namespace AlatrafClinic.Application.Features.People.Patients.Dtos
{
    public class PatientDto
    {
        public int PatientId { get; set; }
        public int PersonId { get; set; }
        public PersonDto? Person { get; set; }
        public PatientType PatientType { get; set; }
        public string? AutoRegistrationNumber { get; set; }
    }
}