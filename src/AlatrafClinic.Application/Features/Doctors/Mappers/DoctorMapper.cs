using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Application.Features.Doctors.Mappers;
using AlatrafClinic.Application.Features.People.Mappers;
using AlatrafClinic.Domain.Departments.DoctorSectionRooms;
using AlatrafClinic.Domain.People.Doctors;

namespace AlatrafClinic.Application.Features.Doctors.Mappers;

public static class DoctorMapper
{
    public static DoctorDto ToDto(this Doctor entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new DoctorDto
        {
            DoctorId = entity.Id,
            PersonDto = entity.Person!.ToDto(),
            DepartmentId = entity.DepartmentId,
            Specialization = entity.Specialization
        };
    }

    public static List<DoctorDto> ToDtos(this IEnumerable<Doctor> entities)
    {
        return entities.Select(e => e.ToDto()).ToList();
    }

    public static TechnicianDto ToTechnicianDto(this DoctorSectionRoom entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new TechnicianDto
        {
            DoctorSectionRoomId = entity.Id,
            DoctorId = entity.DoctorId,
            DoctorName = entity.Doctor.Person?.FullName ?? string.Empty,
            SectionId = entity.SectionId,
            SectionName = entity.Section.Name,
            TodayIndustrialParts = entity.Doctor.TodayIndustrialPartsCount
        };
    }
    public static List<TechnicianDto> ToTechnicianDtos(this IEnumerable<DoctorSectionRoom> entities)
    {
        return entities.Select(e => e.ToTechnicianDto()).ToList();
    }
    public static TherapistDto ToTherapistDto(this DoctorSectionRoom entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new TherapistDto
        {
            DoctorSectionRoomId = entity.Id,
            DoctorId = entity.DoctorId,
            DoctorName = entity.Doctor.Person?.FullName ?? string.Empty,
            SectionId = entity.SectionId,
            SectionName = entity.Section.Name,
            TodaySessions = entity.Doctor.TodaySessionsCount,
            RoomId = entity?.RoomId ?? 0,
            RoomName = entity?.Room?.Name ?? string.Empty,
        };
    }
    public static List<TherapistDto> ToTherapistDtos(this IEnumerable<DoctorSectionRoom> entities)
    {
        return entities.Select(e => e.ToTherapistDto()).ToList();
    }
}