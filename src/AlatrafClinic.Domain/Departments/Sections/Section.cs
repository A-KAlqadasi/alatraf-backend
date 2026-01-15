using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.DoctorSectionRooms;
using AlatrafClinic.Domain.Departments.Sections.Rooms;
using AlatrafClinic.Domain.MedicalPrograms;

namespace AlatrafClinic.Domain.Departments.Sections;


public class Section :AuditableEntity<int>
{
    public string Name { get; private set; } = default!;
    public int DepartmentId { get; private set; }
    public Department Department { get; set; } = default!;
    private readonly List<Room> _rooms = new();
    public IReadOnlyCollection<Room> Rooms => _rooms.AsReadOnly();
    private readonly List<DoctorSectionRoom> _doctorAssignments = new();

    public IReadOnlyCollection<DoctorSectionRoom> DoctorAssignments => _doctorAssignments.AsReadOnly();

    public ICollection<MedicalProgram> MedicalPrograms { get; set; } = new List<MedicalProgram>();

    private Section() { }

    private Section(string name, int departmentId)
    {
        Name = name;
        DepartmentId = departmentId;
    }

    public static Result<Section> Create(string name, int departmentId)
    {
        if (string.IsNullOrWhiteSpace(name))
            return SectionErrors.NameRequired;

        if (departmentId <= 0)
            return SectionErrors.InvalidDepartmentId;

        return new Section(name, departmentId);
    }
    public Result<Updated> UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            return SectionErrors.NameRequired;

        Name = newName;
        return Result.Updated;
    }
    public Result<Room> AddRoom(string roomName)
    {
        if (_rooms.Any(r => r.Name == roomName))
            return SectionErrors.DuplicateRoomName;

        var roomOrError = Room.Create(roomName, Id);
        if (roomOrError.IsError)
            return roomOrError.TopError;

        var room = roomOrError.Value;

        _rooms.Add(room);
        return room;
    }

    public Result<Success> AddRooms(List<string> roomNames)
    {   
        foreach(var room in roomNames)
        {
            var result = AddRoom(room);
            if (result.IsError)
                return result.TopError;
        }
        
        return Result.Success;
    }
}