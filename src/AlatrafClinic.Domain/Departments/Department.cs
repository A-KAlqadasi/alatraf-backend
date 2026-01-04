using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.Sections;
using AlatrafClinic.Domain.People.Doctors;
using AlatrafClinic.Domain.Services;

namespace AlatrafClinic.Domain.Departments;

public class Department : AuditableEntity<int>
{
    private readonly List<Service> _services = new();
    public ICollection<Service> Services => _services.AsReadOnly();
    public string Name { get; private set; } = default!;
    private readonly List<Section> _sections = new();
    public IReadOnlyCollection<Section> Sections => _sections.AsReadOnly();

    // navigation
    public ICollection<Doctor> Doctors = new List<Doctor>();

    private Department() { }

    private Department(int id, string name):base(id)
    {
        Name = name;
    }

    public static Result<Department> Create(int id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return DepartmentErrors.NameRequired;

        return new Department(id, name);
    }

    public Result<Updated> Update(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            return DepartmentErrors.NameRequired;

        Name = newName;
        return Result.Updated;
    }

    public Result<Section> AddSection(string sectionName)
    {
        if (string.IsNullOrWhiteSpace(sectionName))
            return SectionErrors.NameRequired;

        if (_sections.Any(s => s.Name.Equals(sectionName, StringComparison.OrdinalIgnoreCase)))
            return DepartmentErrors.DuplicateSectionName;

        var result = Section.Create(sectionName, Id);
        if (result.IsError)
            return result.Errors;

        var section = result.Value;
        _sections.Add(section);
        return section;
    }
   public Result<Service> AddService(int id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return DepartmentErrors.ServiceRequired;

        if (_services.Any(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            return DepartmentErrors.DuplicateServiceName;

        var result = Service.Create(id, name, Id);
        if (result.IsError)
            return result.Errors;

        var service = result.Value;
        _services.Add(service);
        return service;
    }
}