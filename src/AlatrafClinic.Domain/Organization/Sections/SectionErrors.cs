using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Organization.Sections;

public static class SectionErrors
{
    public static readonly Error NameRequired = Error.Validation(
        code: "Section.NameRequired",
        description: "Section name is required.");
    public static readonly Error DepartmentIdRequired = Error.Validation(
        code: "Section.DepartmentIdRequired",
        description: "Department Id is required.");

}