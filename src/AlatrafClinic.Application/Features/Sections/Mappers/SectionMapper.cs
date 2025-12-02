using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.People.Doctors.Mappers;
using AlatrafClinic.Application.Features.Rooms.Mappers;
using AlatrafClinic.Application.Features.Sections.Dtos;
using AlatrafClinic.Application.Features.Sections.Mappers;
using AlatrafClinic.Domain.Departments.Sections;

namespace AlatrafClinic.Application.Features.Sections.Mappers;

public static class SectionMapper
{
    public static SectionDto ToDto(this Section entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new SectionDto
        {
            Id = entity.Id,
            Name = entity.Name,
            DepartmentId = entity.DepartmentId,
            DepartmentName = entity.Department.Name,
            Rooms = entity.Rooms.Any() ? entity.Rooms.ToDtos() : null,
            Doctors = entity.DoctorAssignments
                        .Select(da => da.Doctor.ToDto())
                        .ToList()
        };
    }

    public static List<SectionDto> ToDtos(this IEnumerable<Section> entities)
    {
        return entities.Select(e => e.ToDto()).ToList();
    }
}