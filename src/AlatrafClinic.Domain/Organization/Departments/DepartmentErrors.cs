using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Organization.Departments;

public static class DepartmentErrors
{
    public static readonly Error NameRequired = Error.Validation(
        code: "Department.NameRequired",
        description: "Department name is required.");
    public static readonly Error NameTooLong = Error.Validation(
        code: "Department.NameTooLong",
        description: "Department name must not exceed 100 characters.");
    
}