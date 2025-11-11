using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Organization.Departments;
using AlatrafClinic.Domain.Organization.DoctorSectionRooms;
using AlatrafClinic.Domain.Organization.Rooms;
using AlatrafClinic.Domain.Organization.Sections;
using AlatrafClinic.Domain.People;

namespace AlatrafClinic.Domain.Organization.Doctors;

public class Doctor : AuditableEntity<int>
{
    private readonly List<DoctorSectionRoom> _assignments = [];
    public int PersonId { get; private set; }
    public Person? Person { get; set; }

    public string? Specialization { get; set; }
    public int DepartmentId { get; private set; }
    public Department Department { get; private set; } = default!;
    public IReadOnlyCollection<DoctorSectionRoom> Assignments => _assignments.AsReadOnly();
    private DoctorSectionRoom? ActiveAssignment => _assignments.FirstOrDefault(a => a.IsActive);
    public DoctorSectionRoom? GetCurrentAssignment() => ActiveAssignment;
    public IReadOnlyCollection<DoctorSectionRoom> GetAssignmentHistory() => _assignments.ToList();

    private Doctor() { }

    private Doctor(int personId, int departmentId, string? specialization)
    {
        PersonId = personId;
        DepartmentId = departmentId;
        Specialization = specialization;
    }

    public static Result<Doctor> Create(int personId, int departmentId, string? specialization)
    {
        if (personId <= 0)
            return DoctorErrors.PersonIdRequired;

        if (departmentId <= 0)
            return DoctorErrors.DepartmentIdRequired;

        return new Doctor(personId, departmentId, specialization);
    }
    public Result<Updated> UpdateSpecialization(string? specialization)
    {

        Specialization = specialization;
        return Result.Updated;
    }
    public Result<Updated> ChangeDepartment(int newDepartmentId)
    {
        if (newDepartmentId <= 0)
            return DoctorErrors.DepartmentIdRequired;

        if (newDepartmentId == DepartmentId)
            return DoctorErrors.SameDepartment;

        if (_assignments.Any(a => a.IsActive && a.Section.DepartmentId == DepartmentId))
            return DoctorErrors.CannotChangeDepartmentWithActiveAssignments;

        DepartmentId = newDepartmentId;
        return Result.Updated;
    }
    public Result<DoctorSectionRoom> AssignToSectionAndRoom(Section section, Room room, string? notes = null)
    {
        if (section.DepartmentId != DepartmentId)
            return DoctorErrors.SectionOutsideDepartment;

        if (room.SectionId != section.Id)
            return DoctorErrors.RoomOutsideSection;

        // End any active assignment before creating new one
        ActiveAssignment?.EndAssignment();

        var newAssignment = DoctorSectionRoom.AssignToRoom(Id, section.Id, room.Id, notes);
        _assignments.Add(newAssignment.Value);

        return newAssignment;
    }




    public Result<DoctorSectionRoom> AssignToRoom(Room room, string? notes = null)
    {
        if (room.Section is null)
            return DoctorErrors.RoomWithoutSection;

        if (room.Section.DepartmentId != DepartmentId)
            return DoctorErrors.RoomOutsideDepartment;

        var active = ActiveAssignment;
        if (active is null)
            return DoctorErrors.NoActiveAssignment;

        // Ensure it's the same section
        if (active.SectionId != room.SectionId)
            return DoctorErrors.RoomOutsideActiveSection;

        // End current assignment and create new one for new room
        active.EndAssignment();

        var newAssignment = DoctorSectionRoom.AssignToRoom(Id, room.SectionId, room.Id, notes);
        _assignments.Add(newAssignment.Value);

        return newAssignment;
    }




}