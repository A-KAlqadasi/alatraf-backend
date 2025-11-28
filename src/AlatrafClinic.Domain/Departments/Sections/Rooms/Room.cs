using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Organization.DoctorSectionRooms;
using AlatrafClinic.Domain.Organization.Sections;

namespace AlatrafClinic.Domain.Organization.Rooms;

public class Room : AuditableEntity<int>
{
    public string Name { get; private set; } = default!;
    public int SectionId { get; private set; }

    public Section Section { get; private set; } = default!;
    private readonly List<DoctorSectionRoom> _doctorAssignments = new();

    public IReadOnlyCollection<DoctorSectionRoom> DoctorAssignments => _doctorAssignments.AsReadOnly();

    private Room() { }

    private Room(string name, int sectionId)
    {
        Name = name;
        SectionId = sectionId;

    }

    public static Result<Room> Create(string name, int sectionId)
    {
        if (string.IsNullOrWhiteSpace(name))
            return RoomErrors.InvalidName;

        if (sectionId <= 0)
            return RoomErrors.InvalidSection;

        return new Room(name, sectionId);
    }

    public Result<Updated> UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            return RoomErrors.InvalidName;

        if (Section.Rooms.Any(r => r.Id != Id && r.Name == newName))
            return RoomErrors.DuplicateRoomName;

        Name = newName;
        return Result.Updated;
    }
}