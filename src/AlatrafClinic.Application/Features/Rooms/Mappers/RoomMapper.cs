using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.People.Doctors.Mappers;
using AlatrafClinic.Application.Features.Rooms.Dtos;
using AlatrafClinic.Application.Features.Rooms.Mappers;
using AlatrafClinic.Domain.Departments.Sections.Rooms;

namespace AlatrafClinic.Application.Features.Rooms.Mappers;

public static class RoomMapper
{
    public static RoomDto ToDto(this Room entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new RoomDto
        {
            Id = entity.Id,
            Name = entity.Name,
            SectionId = entity.SectionId,
            SectionName = entity.Section.Name,
            Doctors = entity.DoctorAssignments
                        .Select(da => da.Doctor.ToDto())
                        .ToList()
        };
    }

    public static List<RoomDto> ToDtos(this IEnumerable<Room> entities)
    {
        return entities.Select(e => e.ToDto()).ToList();
    }
}