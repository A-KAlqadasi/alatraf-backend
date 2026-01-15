using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.People;

public class Address : AuditableEntity<int>
{
    public string Name { get; set; } = null!;
    public ICollection<Person> People { get; set; } = new List<Person>();
    
    private Address(int id, string name):base(id)
    {
        Name = name;
    }
    
    public static Result<Address> Create(int id, string name)
    {
       return new Address(id, name);
    }
    
    public Result<Updated> Update(string name)
    {
        Name = name;
        
        return Result.Updated;
    }
}