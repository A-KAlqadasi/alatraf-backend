using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Services;

public static class ServiceErrors
{
    public static readonly Error NameIsRequired = Error.Validation("Service.NameIsRequired", "اسم الخدمة مطلوب.");
    public static readonly Error DepartmentIdIsRequired = Error.Validation("Service.DepartmentIdIsRequired", "معرف الإدارة مطلوب.");
    public static readonly Error ServiceNotFound = Error.NotFound("Service.NotFound", "الخدمة المحددة غير موجودة.");
    public static readonly Error InvalidServicePrice = Error.Validation("Service.InvalidServicePrice", "يجب أن يكون سعر الخدمة أكبر من صفر.");
    public static readonly Error ServiceIdInvalid = Error.Validation("Service.InvalidServiceId", "معرف الخدمة غير صالح.");
}