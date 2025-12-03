using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.DoctorSectionRooms;
using AlatrafClinic.Domain.Departments.Sections.Rooms;
using AlatrafClinic.Domain.TherapyCards.MedicalPrograms;

namespace AlatrafClinic.Domain.Departments.Sections;


public class Section :AuditableEntity<int>
{
    public string Name { get; private set; } = default!;
    public int DepartmentId { get; private set; }
    public Department Department { get; private set; } = default!;
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

        if (Department.Sections.Any(s => s.Id != Id && 
                                        s.Name.Equals(newName, StringComparison.OrdinalIgnoreCase)))
            return SectionErrors.DuplicateSectionName;

        Name = newName;
        return Result.Updated;
    }
}