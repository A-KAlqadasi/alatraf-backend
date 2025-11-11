using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Identity;

namespace AlatrafClinic.Domain.People.Employees;

public sealed class Employee : AuditableEntity<Guid>
{
    public int PersonId { get; set; }
    public Person? Person { get; set; }
    public Role Role { get; set; }
    private Employee() { }

    private Employee(Guid id, int personId, Role role):base(id)
    {
        PersonId = personId;
        Role = role;
    }

    public static Result<Employee> Create( int personId, Role role)
    {
       
        
        if (personId <= 0)
        {
            return EmployeeErrors.PersonIdRequired;
        }

        if (!Enum.IsDefined(role))
        {
            return EmployeeErrors.RoleInvalid;
        }
        var employeeId = Guid.NewGuid();

         if (employeeId == Guid.Empty)
        {
            return EmployeeErrors.IdRequired;
        }
        
        return new Employee(employeeId,personId, role);
    }
   public Result<Updated> UpdateRole(Role newRole)
    {
        if (!Enum.IsDefined(typeof(Role), newRole))
            return EmployeeErrors.RoleInvalid;

        if (Role == newRole)  return EmployeeErrors.SameRole;

        Role = newRole;
        return Result.Updated;
    }
}