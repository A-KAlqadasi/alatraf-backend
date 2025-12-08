using AlatrafClinic.Application.Features.People.Dtos;
using AlatrafClinic.Domain.Patients.Enums;

namespace AlatrafClinic.Application.Features.Patients.Dtos;

public class PatientDto
{
    public int PatientId { get; set; }
    public int PersonId { get; set; }
    public PersonDto? PersonDto { get; set; }
    public PatientType PatientType { get; set; }
}
