
using AlatrafClinic.Application.Features.Organization.Departments.Dtos;
using AlatrafClinic.Application.Features.Organization.Sections.Mappers;
using AlatrafClinic.Domain.Organization.Departments;

namespace AlatrafClinic.Application.Features.Organization.Departments.Mappers;

public static class DepartmentMapper
{
    public static DepartmentDto ToDto(this Department entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new DepartmentDto(
            Id: entity.Id,
            Name: entity.Name,
            Sections: entity.Sections.ToDtos()
        );
    }

    public static List<DepartmentDto> ToDtos(this IEnumerable<Department> entities)
    {
        return entities.Select(e => e.ToDto()).ToList();
    }
}