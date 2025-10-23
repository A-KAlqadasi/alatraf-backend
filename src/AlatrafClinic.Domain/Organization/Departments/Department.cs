using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Organization.Doctors;
using AlatrafClinic.Domain.Organization.Sections;

namespace AlatrafClinic.Domain.Organization.Departments;

public class Department:AuditableEntity<int>
{
    public string? Name { get; set; }
    public ICollection<Section> Sections { get; set; } = new List<Section>();
    // public ICollection<Services> Services { get; set; } = new List<Services>();
    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    
    private Department()
    {

    }
    private Department(string name)
    {
        Name = name;
    }

    public static Result<Department> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return DepartmentErrors.NameRequired;
        }

        if (name.Length > 100)
        {
            return DepartmentErrors.NameTooLong;
        }
        return new Department(name);
    }

    public Result<Updated> Update(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return DepartmentErrors.NameRequired;
        }
        
        if (name.Length > 100)
        {
            return DepartmentErrors.NameTooLong;
        }
        
        Name = name;
        return Result.Updated;
    }
}