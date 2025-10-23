using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Organization.Departments;
using AlatrafClinic.Domain.Organization.DoctorSectionRooms;
using AlatrafClinic.Domain.Organization.Rooms;

namespace AlatrafClinic.Domain.Organization.Sections;


public class Section :AuditableEntity<int>
{
    public string? Name { get; set; }
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

    public ICollection<Room> Rooms { get; set; } = new List<Room>();
    public ICollection<DoctorSectionRoom> DoctorSectionRooms { get; set; } = new List<DoctorSectionRoom>();

    private Section() { }

    private Section(string name, int? departmentId)
    {
        Name = name;
        DepartmentId = departmentId;
    }
    public static Result<Section> Create(string name, int? departmentId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return SectionErrors.NameRequired;
        }

        if (departmentId is null || departmentId <= 0)
        {
            return SectionErrors.DepartmentIdRequired;
        }

        return new Section(name, departmentId);
    }

    public Result<Updated> Update(string name, int? departmentId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return SectionErrors.NameRequired;
        }

        if (departmentId is null || departmentId <= 0)
        {
            return SectionErrors.DepartmentIdRequired;
        }

        Name = name;
        DepartmentId = departmentId;

        return Result.Updated;
    }
}