using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Departments;

public static class DepartmentErrors
{
    public static readonly Error NameRequired = Error.Validation(
        code: "Department.NameRequired",
        description: "Department name is required.");
    public static readonly Error ServiceRequired = Error.Validation(
        code: "Department.ServiceRequired",
        description: "At least one service is required.");
    public static readonly Error DuplicateSectionName = Error.Conflict(
        code: "Department.DuplicateSectionName",
        description: "A section with the same name already exists in this department.");

    public static readonly Error DuplicateServiceName = Error.Conflict(
        code: "Department.DuplicateServiceName",
        description: "A service with the same name already exists in this department.");
    public static readonly Error NotFound = Error.NotFound("Department.NotFound", "Department is not found");
}
