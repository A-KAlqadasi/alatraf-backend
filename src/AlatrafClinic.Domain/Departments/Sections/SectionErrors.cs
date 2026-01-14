using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Departments.Sections;

public static class SectionErrors
{
    public static readonly Error NameRequired = Error.Validation(
        code: "Section.NameRequired",
        description: "اسم القسم مطلوب");
    public static readonly Error InvalidDepartmentId = Error.Validation(
        code: "Section.InvalidDepartmentId",
        description: "رقم الادارة غير صالح");
    public static readonly Error DuplicateSectionName = Error.Conflict(
        code: "Section.DuplicateSectionName",
        description: "يوجد قسم بنفس الاسم في هذه الادارة");
    public static readonly Error SectionNotFound = Error.NotFound(
        code: "Section.NotFound",
        description: "القسم غير موجود");
    public static readonly Error DuplicateRoomName = Error.Conflict(
        code: "Section.DuplicateRoomName",
        description: "يوجد غرفة بنفس الاسم في هذا القسم");
}
