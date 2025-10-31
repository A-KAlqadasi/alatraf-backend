using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;

using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Organization.Departments;
using AlatrafClinic.Domain.Services.Tickets;

namespace AlatrafClinic.Domain.Services;

public class Service : AuditableEntity<int>
{
    public string Name { get; private set; } = string.Empty;
    public string? Code { get; private set; }
    public int? DepartmentId { get; private set; }
    public Department? Department { get; set; }
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    private Service()
    {
    }

    private Service(string name, int? departmentId)
    {
        Name = name;
        DepartmentId = departmentId;
    }

    public static Result<Service> Create(string name, int? departmentId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ServiceErrors.NameIsRequired;
        }
        if (departmentId is null || departmentId <= 0)
        {
            return ServiceErrors.DepartmentIdIsRequired;
        }
        
        return new Service(name, departmentId);
    }
    
     public  Result<Updated> Update(string? name, int? departmentId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ServiceErrors.NameIsRequired;
        }
        if (departmentId is null || departmentId <= 0)
        {
            return ServiceErrors.DepartmentIdIsRequired;
        }
        Name = name;
        DepartmentId = departmentId;

        return Result.Updated;
    }
}