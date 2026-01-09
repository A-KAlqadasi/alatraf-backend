using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.People;

public class Address : AuditableEntity<int>
{
    public string Name { get; set; } = null!;
    private Address(string name)
    {
        Name = name;
    }
    
    public static Result<Address> Create(string name)
    {
       return new Address(name);
    }
    public Result<Updated> Update(string name)
    {
        Name = name;
        
        return Result.Updated;
    }
}