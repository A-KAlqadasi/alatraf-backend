using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Organization.Doctors;
using AlatrafClinic.Domain.Organization.Rooms;
using AlatrafClinic.Domain.Organization.Sections;

namespace AlatrafClinic.Domain.Organization.DoctorSectionRooms;

public class DoctorSectionRoom : AuditableEntity<int>
{
    public int? DoctorId { get; set; }
    public Doctor? Doctor { get; set; }
    public int? SectionId { get; set; }
    public Section? Section { get; set; }
    public int? RoomId { get; set; }
    public Room? Room { get; set; }
    public DateTime? AssignDate { get; set; }
    public bool? IsActive { get; set; }
    public string? Notes { get; set; }

    private DoctorSectionRoom() { }

    private DoctorSectionRoom(int? doctorId, int? sectionId, int? roomId, DateTime? assignDate, bool? isActive, string? notes)
    {
        DoctorId = doctorId;
        SectionId = sectionId;
        RoomId = roomId;
        AssignDate = assignDate;
        IsActive = isActive;
        Notes = notes;
    }

    public static Result<DoctorSectionRoom> Create(int? doctorId, int? sectionId, int? roomId, DateTime? assignDate, bool? isActive, string? notes)
    {
        if (doctorId is null || doctorId <= 0)
        {
            return DoctorSectionRoomErrors.DoctorIdRequired;
        }

        if (sectionId is null || sectionId <= 0)
        {
            return DoctorSectionRoomErrors.SectionIdRequired;
        }

        if (roomId is null || roomId <= 0)
        {
            return DoctorSectionRoomErrors.RoomIdRequired;
        }

        if (assignDate is null)
        {
            return DoctorSectionRoomErrors.AssignDateRequired;
        }

        if (isActive is null)
        {
            return DoctorSectionRoomErrors.IsActiveRequired;
        }

        if (assignDate is not null && assignDate < DateTime.UtcNow)
        {
            return DoctorSectionRoomErrors.AssignDateInvalid;
        }

        return new DoctorSectionRoom(doctorId, sectionId, roomId, assignDate, isActive, notes);
    }

    public Result<Updated> Update(int? doctorId, int? sectionId, int? roomId, DateTime? assignDate, bool? isActive, string? notes)
    {
        if (doctorId is null || doctorId <= 0)
        {
            return DoctorSectionRoomErrors.DoctorIdRequired;
        }

        if (sectionId is null || sectionId <= 0)
        {
            return DoctorSectionRoomErrors.SectionIdRequired;
        }

        if (roomId is null || roomId <= 0)
        {
            return DoctorSectionRoomErrors.RoomIdRequired;
        }

        if (assignDate is null)
        {
            return DoctorSectionRoomErrors.AssignDateRequired;
        }

        if (isActive is null)
        {
            return DoctorSectionRoomErrors.IsActiveRequired;
        }

        if (assignDate is not null && assignDate < DateTime.UtcNow)
        {
            return DoctorSectionRoomErrors.AssignDateInvalid;
        }

        DoctorId = doctorId;
        SectionId = sectionId;
        RoomId = roomId;
        AssignDate = assignDate;
        IsActive = isActive;
        Notes = notes;

        return Result.Updated;
    }
}