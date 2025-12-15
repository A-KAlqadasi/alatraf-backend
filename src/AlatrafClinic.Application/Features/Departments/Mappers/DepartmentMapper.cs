using AlatrafClinic.Application.Features.Departments.Dtos;
using AlatrafClinic.Application.Features.Departments.Mappers;
using AlatrafClinic.Application.Features.Sections.Mappers;
using AlatrafClinic.Domain.Departments;

namespace AlatrafClinic.Application.Features.Departments.Mappers;

public static class DepartmentMapper
{
    public static DepartmentDto ToDto(this Department entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new DepartmentDto(
            Id: entity.Id,
            Name: entity.Name,
            Sections: entity.Sections.Count() > 0 ? entity.Sections.ToDtos() : null
        );
    }

    public static List<DepartmentDto> ToDtos(this IEnumerable<Department> entities)
    {
        return entities.Select(e => e.ToDto()).ToList();
    }
}