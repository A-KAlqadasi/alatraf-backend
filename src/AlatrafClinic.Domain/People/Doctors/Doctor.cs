using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments;
using AlatrafClinic.Domain.Departments.DoctorSectionRooms;
using AlatrafClinic.Domain.Departments.Sections;
using AlatrafClinic.Domain.Departments.Sections.Rooms;

namespace AlatrafClinic.Domain.People.Doctors;

public class Doctor : AuditableEntity<int>
{
    public int PersonId { get; private set; }
    public Person? Person { get; set; }
    public string? Specialization { get; set; }
    public int DepartmentId { get; private set; }
    public Department Department { get; private set; } = default!;
    public bool IsActive { get; private set; } = true; 
    private readonly List<DoctorSectionRoom> _assignments = [];
    public IReadOnlyCollection<DoctorSectionRoom> Assignments => _assignments.AsReadOnly();
    private DoctorSectionRoom? ActiveAssignment => _assignments.SingleOrDefault(a => a.IsActive);
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

        if (ActiveAssignment is not null && ActiveAssignment.SectionId != section.Id && ActiveAssignment.RoomId != room.Id)
        {
            return DoctorErrors.DoctorHasSessionsToday;
        }
        
        ActiveAssignment?.EndAssignment();

        var newAssignment = DoctorSectionRoom.AssignToRoom(Id, section.Id, room.Id, notes);
        _assignments.Add(newAssignment.Value);

        return newAssignment;
    }

    public Result<DoctorSectionRoom> AssignToSection(Section section, string? notes = null)
    {

        if (section is null)
            return DoctorErrors.RoomWithoutSection;

        if (section.DepartmentId != DepartmentId)
            return DoctorErrors.SectionOutsideDepartment;

        if (ActiveAssignment is not null && ActiveAssignment.SectionId != section.Id)
        {
            return DoctorErrors.DoctorHasIndustrialPartsToday;
        }
        
        ActiveAssignment?.EndAssignment();
        
        var newAssignment = DoctorSectionRoom.AssignToSection(Id, section.Id, notes);

        _assignments.Add(newAssignment.Value);

        return newAssignment;
    }

    public void Activate()
    {
        IsActive = true;
    }
    public void DeActivate()
    {
        IsActive = false;
    }
}