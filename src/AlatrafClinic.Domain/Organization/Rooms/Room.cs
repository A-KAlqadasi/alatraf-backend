using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Organization.DoctorSectionRooms;
using AlatrafClinic.Domain.Organization.Sections;

namespace AlatrafClinic.Domain.Organization.Rooms;

public class Room : AuditableEntity<int>
{  
    public string? Name { get; set; }
    public int? SectionId { get; set; }
    public Section? Section { get; set; }
    public ICollection<DoctorSectionRoom> DoctorSectionRooms { get; set; } = new List<DoctorSectionRoom>();
    
    private Room()
    {
    }

    private Room(string? name, int? sectionId)
    {
        Name = name;
        SectionId = sectionId;
    }
    public static Result<Room> Create(string? name, int? sectionId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return RoomErrors.NameRequired;
        }

        if (sectionId is null || sectionId <= 0)
        {
            return RoomErrors.SectionIdRequired;
        }

        return new Room(name, sectionId);
    }

    public Result<Updated> Update(string? name, int? sectionId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return RoomErrors.NameRequired;
        }

        if (sectionId is null || sectionId <= 0)
        {
            return RoomErrors.SectionIdRequired;
        }

        Name = name;
        SectionId = sectionId;

        return Result.Updated;
    }

}