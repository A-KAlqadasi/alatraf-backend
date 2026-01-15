using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.People.Doctors;


public static class DoctorErrors
{
    public static readonly Error DepartmentIdRequired =
        Error.Validation("Doctor.DepartmentIdRequired", "معرف الادارة مطلوب");

    public static readonly Error SectionOutsideDepartment =
        Error.Validation("Doctor.SectionOutsideDepartment", "القسم لا ينتمي إلى ادارة الطبيب.");
    public static readonly Error RoomWithoutSection =
        Error.Validation("Doctor.RoomWithoutSection", "الغرفة يجب أن تنتمي إلى قسم صالح.");
    public static readonly Error CannotChangeDepartmentWithAssignments =
    Error.Conflict("Doctor.CannotChangeDepartmentWithAssignments",
         "لا يمكن للطبيب تغيير الإدارة أثناء وجود تعيينات.");
    public static readonly Error RoomOutsideSection = Error.Validation(
          code: "Doctor.RoomOutsideSection",
          description: "الغرفة المحددة لا تنتمي إلى القسم المقدم.");

    public static readonly Error NoActiveAssignment = Error.Validation(
        code: "Doctor.NoActiveAssignment",
        description: "لا يوجد تعيين نشط للطبيب لتحديثه أو تعديله.");

    public static readonly Error DoctorHasIndustrialPartsToday = Error.Conflict(
        code: "Doctor.DoctorHasIndustrialPartsToday",
        description: "للطبيب أجزاء صناعية مسجلة لليوم ولا يمكن إلغاء تعيينه من القسم.");
    
    public static readonly Error DoctorHasSessionsToday = Error.Conflict(
        code: "Doctor.DoctorHasSessionsToday",
        description: "للطبيب جلسات مجدولة لليوم ولا يمكن إلغاء تعيينه من القسم/الغرفة.");
    public static readonly Error NotFound = Error.NotFound("Doctor.NotFound", "الطبيب غير موجود");
}
