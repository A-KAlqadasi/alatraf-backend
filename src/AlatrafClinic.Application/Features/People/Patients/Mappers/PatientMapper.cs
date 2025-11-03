

using AlatrafClinic.Application.Features.People.Patients.Dtos;
using AlatrafClinic.Application.Features.People.Persons.Mappers;
using AlatrafClinic.Domain.Patients;

namespace AlatrafClinic.Application.Features.People.Patients.Mappers;

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
            AutoRegistrationNumber = entity.AutoRegistrationNumber
        };
    }

    public static List<PatientDto> ToDtos(this IEnumerable<Patient> entities)
    {
        return entities.Select(e => e.ToDto()).ToList();
    }
}