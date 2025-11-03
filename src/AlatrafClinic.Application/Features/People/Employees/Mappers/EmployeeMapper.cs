

using AlatrafClinic.Application.Features.People.Employees.Dtos;
using AlatrafClinic.Application.Features.People.Persons.Mappers;
using AlatrafClinic.Domain.People.Employees;

namespace AlatrafClinic.Application.Features.People.Employees.Mappers;

public static class EmployeeMapper
{
    public static EmployeeDto ToDto(this Employee entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new EmployeeDto
        {
            EmployeeId = entity.Id,
            PersonId = entity.PersonId,
            Person = entity.Person!.ToDto(),
            Role = entity.Role
        };
    }

    public static List<EmployeeDto> ToDtos(this IEnumerable<Employee> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        return entities.Select(e => e.ToDto()).ToList();
    }
}