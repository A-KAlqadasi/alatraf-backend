using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.People.Employees;


public static class EmployeeErrors
{
    public static readonly Error IdRequired =
       Error.Validation("Employee.IdRequired", "Employee Id is required.");
        
    public static readonly Error PersonIdRequired =
        Error.Validation("Employee.PersonIdRequired", "Employee PersonId is required.");

    public static Error RoleInvalid =>
        Error.Validation("Employee.RoleInvalid", "Invalid role assigned to employee.");
         public static readonly Error SameRole =
        Error.Conflict("Employee.SameRole", "The new role must be different from the current one.");

}