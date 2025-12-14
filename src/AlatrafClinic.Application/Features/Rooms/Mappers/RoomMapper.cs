using AlatrafClinic.Application.Features.Rooms.Dtos;
using AlatrafClinic.Domain.Departments.Sections.Rooms;

namespace AlatrafClinic.Application.Features.Rooms.Mappers;

public static class RoomMapper
{
    public static RoomDto ToDto(this Room room)
    {
        ArgumentNullException.ThrowIfNull(room);

         var section = room.Section;
        var department = section?.Department;

        return new RoomDto
        {
            Id              = room.Id,
            Name            = room.Name,
            SectionId       = room.SectionId,
            SectionName     = section?.Name ?? string.Empty,
            DepartmentId    = section?.DepartmentId ?? 0,
            DepartmentName  = department?.Name ?? string.Empty,
        };
    }

    public static List<RoomDto> ToDtos(this IEnumerable<Room> entities)
    {
        return entities.Select(e => e.ToDto()).ToList();
    }
}