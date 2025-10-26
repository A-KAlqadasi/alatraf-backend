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
    public static readonly Error SectionRequired = Error.Validation(
        code: "Department.SectionRequired",
        description: "At least one section is required.");
    public static readonly Error ServiceRequired = Error.Validation(
        code: "Department.ServiceRequired",
        description: "At least one service is required.");
    public static readonly Error DoctorRequired = Error.Validation(
        code: "Department.DoctorRequired",
        description: "At least one doctor is required.");
}