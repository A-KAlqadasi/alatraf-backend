using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Organization.Doctors;
using AlatrafClinic.Domain.Organization.Sections;
using AlatrafClinic.Domain.Services;

namespace AlatrafClinic.Domain.Organization.Departments;

public class Department : AuditableEntity<int>
{
    private readonly List<Service> _services = new();
    public ICollection<Service> Services => _services.AsReadOnly();

    public string Name { get; private set; } = default!;
    private readonly List<Section> _sections = new();
    public IReadOnlyCollection<Section> Sections => _sections.AsReadOnly();

    private Department() { }

    private Department(string name)
    {
        Name = name;
    }

    public static Result<Department> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return DepartmentErrors.NameRequired;

        return new Department(name);
    }

    public Result<Updated> Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            return DepartmentErrors.NameRequired;

        Name = newName;
        return Result.Updated;
    }

    public Result<Section> AddSection(string sectionName)
    {
        var section = Section.Create(sectionName, Id).Value;
        _sections.Add(section);

        return section;
    }
    public Result<Service> AddService(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return DepartmentErrors.ServiceRequired;

        var service = Service.Create(name, Id).Value;

        _services.Add(service);
        return service;
    }
}