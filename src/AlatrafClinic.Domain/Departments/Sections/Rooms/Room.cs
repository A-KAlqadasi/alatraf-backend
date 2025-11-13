using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Organization.DoctorSectionRooms;
using AlatrafClinic.Domain.Organization.Sections;

namespace AlatrafClinic.Domain.Organization.Rooms;

public class Room : AuditableEntity<int>
{
    public int Number { get; private set; }
    public int SectionId { get; private set; }
    public bool IsDeleted { get; private set; }

    public Section Section { get; private set; } = default!;
    private readonly List<DoctorSectionRoom> _doctorAssignments = new();

    public IReadOnlyCollection<DoctorSectionRoom> DoctorAssignments => _doctorAssignments.AsReadOnly();

    private Room() { }

    private Room(int number, int sectionId)
    {
        Number = number;
        SectionId = sectionId;
        IsDeleted = false;

    }

    public static Result<Room> Create(int number, int sectionId)
    {
        if (number <= 0)
            return RoomErrors.InvalidNumber;

        if (sectionId <= 0)
            return RoomErrors.InvalidSection;

        return new Room(number, sectionId);
    }

    public Result<Updated> UpdateNumber(int newNumber)
    {
        if (newNumber <= 0)
            return RoomErrors.InvalidNumber;

        if (Section.Rooms.Any(r => r.Id != Id && r.Number == newNumber))
            return RoomErrors.DuplicateRoomNumber;

        Number = newNumber;
        return Result.Updated;
    }

     // ✅ Domain operation for soft delete
    public Result<Deleted> SoftDelete()
    {
        if (IsDeleted)
            return RoomErrors.AlreadyDeleted;

        IsDeleted = true;
        return Result.Deleted;
    }

    // ✅ Optional undo
    public Result<Updated> Restore()
    {
        if (!IsDeleted)
            return RoomErrors.NotDeleted;

        IsDeleted = false;
        return Result.Updated;
    }
}