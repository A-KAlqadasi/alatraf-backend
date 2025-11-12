
using AlatrafClinic.Application.Features.Organization.Rooms.Mappers;
using AlatrafClinic.Application.Features.Organization.Sections.Mappers;
using AlatrafClinic.Application.Features.People.Doctors.Dtos;
using AlatrafClinic.Application.Features.People.Persons.Mappers;
using AlatrafClinic.Domain.Organization.Doctors;
using AlatrafClinic.Domain.Organization.DoctorSectionRooms;
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
    public static DoctorSectionRoomDto ToDto(this DoctorSectionRoom entity)
    {
        return new DoctorSectionRoomDto
        {
            DoctorSectionRoomId = entity.Id,
            Doctor = entity.Doctor.ToDto(),
            Section = entity.Section.ToDto(),
            Room = entity.Room?.ToDto(),
            IsActive = entity.IsActive,
            AssignDate = entity.AssignDate,
            EndAssignDate = entity.EndDate
        };
    }
    
}