using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Organization.DoctorSectionRooms;
using AlatrafClinic.Domain.Organization.Sections;

namespace AlatrafClinic.Domain.Organization.Rooms;

public class Room : AuditableEntity<int>
{  
   private readonly List<DoctorSectionRoom> _doctorAssignments = new();
    public int Number { get; private set; }
    public int SectionId { get; private set; }
    public Section Section { get; private set; } = default!;
    public IReadOnlyCollection<DoctorSectionRoom> DoctorAssignments => _doctorAssignments.AsReadOnly();

    private Room() { }

    private Room(int number, int sectionId)
    {
        Number = number;
        SectionId = sectionId;
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

        Number = newNumber;
        return Result.Updated;
    }
}