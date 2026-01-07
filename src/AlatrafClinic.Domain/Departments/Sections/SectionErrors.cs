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
    public static readonly Error DuplicateSectionName = Error.Conflict(
        code: "Section.DuplicateSectionName",
        description: "Another section with the same name already exists in this department.");
    public static readonly Error SectionNotFound = Error.NotFound(
        code: "Section.NotFound",
        description: "Section is not found.");
    public static readonly Error DuplicateRoomName = Error.Conflict(
        code: "Section.DuplicateRoomName",
        description: "Another room with the same name already exists in this section.");
}
