using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Departments.Sections;

public static class SectionErrors
{
    public static readonly Error NameRequired = Error.Validation(
        code: "Section.NameRequired",
        description: "Section name is required.");
    public static readonly Error InvalidDepartmentId = Error.Validation(
        code: "Section.InvalidDepartmentId",
        description: "Department Id is invalid.");
    public static readonly Error RoomRequired = Error.Validation(
        code: "Section.RoomRequired",
        description: "At least one room is required.");
    public static readonly Error DuplicateSectionName = Error.Validation(
        code: "Section.DuplicateSectionName",
        description: "Another section with the same name already exists in this department.");
    public static readonly Error SectionNotFound = Error.NotFound(
        code: "Section.NotFound",
        description: "Section is not found.");
}
