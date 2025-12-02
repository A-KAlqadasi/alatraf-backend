using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Departments;

public static class DepartmentErrors
{
    public static readonly Error NameRequired = Error.Validation(
        code: "Department.NameRequired",
        description: "Department name is required.");
    public static readonly Error NameTooLong = Error.Validation(
        code: "Department.NameTooLong",
        description: "Department name must not exceed 100 characters.");
    public static readonly Error SectionRequired = Error.Validation(
        code: "Department.SectionRequired",
        description: "At least one section is required.");
    public static readonly Error ServiceRequired = Error.Validation(
        code: "Department.ServiceRequired",
        description: "At least one service is required.");
    public static readonly Error DoctorRequired = Error.Validation(
        code: "Department.DoctorRequired",
        description: "At least one doctor is required.");
    public static readonly Error DuplicateSectionName = Error.Validation(
        code: "Department.DuplicateSectionName",
        description: "A section with the same name already exists in this department.");

    public static readonly Error DuplicateServiceName = Error.Validation(
        code: "Department.DuplicateServiceName",
        description: "A service with the same name already exists in this department.");
    public static readonly Error NotFound = Error.NotFound("Department.NotFound", "Department is not found");
}
