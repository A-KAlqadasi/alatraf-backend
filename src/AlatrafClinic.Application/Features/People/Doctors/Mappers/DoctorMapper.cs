


using AlatrafClinic.Application.Features.People.Doctors.Dtos;
using AlatrafClinic.Application.Features.People.Persons.Mappers;
using AlatrafClinic.Domain.Organization.Doctors;
using AlatrafClinic.Domain.People;

namespace AlatrafClinic.Application.Features.People.Doctors.Mappers;

public static class DoctorMapper
{
    public static DoctorDto ToDto(this Doctor entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new DoctorDto
        {
            DoctorId = entity.Id,
            PersonId = entity.PersonId,
            PersonDto = entity.Person!.ToDto(),
            DepartmentId = entity.DepartmentId,
            Specialization = entity.Specialization
        };
    }

    public static List<DoctorDto> ToDtos(this IEnumerable<Doctor> entities)
    {
        return entities.Select(e => e.ToDto()).ToList();
    }
}