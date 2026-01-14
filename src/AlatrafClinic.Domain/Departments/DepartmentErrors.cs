using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Departments;

public static class DepartmentErrors
{
    public static readonly Error NameRequired = Error.Validation(
        code: "Department.NameRequired",
        description: "اسم الادارة مطلوب");
    public static readonly Error ServiceRequired = Error.Conflict(
        code: "Department.ServiceRequired",
        description: "يتطلب وجود خدمة واحدة على الأقل");
    public static readonly Error DuplicateSectionName = Error.Conflict(
        code: "Department.DuplicateSectionName",
        description: "يوجد قسم بنفس الاسم في الادارة");

    public static readonly Error DuplicateServiceName = Error.Conflict(
        code: "Department.DuplicateServiceName",
        description: "يوجد خدمة بنفس الاسم في الادارة");
    public static readonly Error NotFound = Error.NotFound("Department.NotFound", "الادارة غير موجودة");
}
