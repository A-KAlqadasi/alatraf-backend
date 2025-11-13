

using AlatrafClinic.Application.Features.Organization.Rooms.Mappers;
using AlatrafClinic.Application.Features.Organization.Sections.Dtos;
using AlatrafClinic.Domain.Organization.Sections;


namespace AlatrafClinic.Application.Features.Organization.Sections.Mappers;

public static class SectionMapper
{
    public static SectionDto ToDto(this Section entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new SectionDto(
           entity.Id,
            Name: entity.Name,
            DepartmentId: entity.DepartmentId,
            Rooms: entity.Rooms.ToDtos()
        );
    }

    public static List<SectionDto> ToDtos(this IEnumerable<Section> entities)
    {
        return entities.Select(e => e.ToDto()).ToList();
    }
}