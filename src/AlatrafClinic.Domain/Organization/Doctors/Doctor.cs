using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Organization.Departments;
using AlatrafClinic.Domain.Organization.DoctorSectionRooms;
using AlatrafClinic.Domain.People;

namespace AlatrafClinic.Domain.Organization.Doctors;

public class Doctor : AuditableEntity<int>
{
    private readonly List<DoctorSectionRoom> _assignments = [];
    public int PersonId { get; private set; }
    public int DepartmentId { get; private set; }
    public Department Department { get; private set; } = default!;
    public IReadOnlyCollection<DoctorSectionRoom> Assignments => _assignments.AsReadOnly();

    private Doctor() { }

    private Doctor(int personId, int departmentId)
    {
        PersonId = personId;
        DepartmentId = departmentId;
    }

    public static Result<Doctor> Create(int personId, int departmentId)
    {
        if (personId <= 0)
            return DoctorErrors.PersonIdRequired;

        if (departmentId <= 0)
            return DoctorErrors.DepartmentIdRequired;

        return new Doctor(personId, departmentId);
    }

    public Result<Updated> ChangeDepartment(int newDepartmentId)
    {
        if (newDepartmentId <= 0)
            return DoctorErrors.DepartmentIdRequired;

        DepartmentId = newDepartmentId;
        return Result.Updated;
    }

    public Result<DoctorSectionRoom> AssignToRoom(int sectionId, int roomId, string? notes = null)
    {
        var current = _assignments.FirstOrDefault(a => a.IsActive);
        current?.EndAssignment();

        var newAssignment = DoctorSectionRoom.AssignToRoom(Id, sectionId, roomId, notes);
        _assignments.Add(newAssignment.Value);

        return newAssignment;
    }

    public Result<DoctorSectionRoom> AssignToSection(int sectionId, string? notes = null)
    {
        var current = _assignments.FirstOrDefault(a => a.IsActive);
        current?.EndAssignment();

        var newAssignment = DoctorSectionRoom.AssignToSection(Id, sectionId, notes);
        _assignments.Add(newAssignment.Value);

        return newAssignment;
    }
}