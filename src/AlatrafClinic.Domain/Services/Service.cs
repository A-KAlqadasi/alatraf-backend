using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments;
using AlatrafClinic.Domain.Services.Tickets;

namespace AlatrafClinic.Domain.Services;

public class Service : AuditableEntity<int>
{
    public string Name { get; private set; } = string.Empty;
    public string? Code { get; private set; }
    public int? DepartmentId { get; private set; }
    public Department? Department { get; set; }
    public decimal? Price { get; private set; } = null;
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    private Service()
    {
    }

    private Service(string name, int? departmentId, decimal? price = null)
    {
        Name = name;
        DepartmentId = departmentId;
        Price = price;
    }

    public static Result<Service> Create(string name, int? departmentId, decimal? price = null)
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

     public  Result<Updated> Update(string? name, int? departmentId, decimal? price = null)
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
        Price = price;
        return Result.Updated;
    }
}