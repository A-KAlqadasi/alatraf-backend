using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.Patients.Dtos;
using AlatrafClinic.Application.Features.Patients.Mappers;
using AlatrafClinic.Application.Features.People.Mappers;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.People;

namespace AlatrafClinic.Application.Features.Patients.Mappers;

public static class PatientMapper 
{
    public static PatientDto ToDto(this Patient entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new PatientDto
        {
            PatientId = entity.Id,
            PersonId = entity.PersonId,
            PersonDto = entity.Person!.ToDto(),
            PatientType = entity.PatientType,
        };
    }

    public static List<PatientDto> ToDtos(this IEnumerable<Patient> entities)
    {
        return entities.Select(e => e.ToDto()).ToList();
    }
}