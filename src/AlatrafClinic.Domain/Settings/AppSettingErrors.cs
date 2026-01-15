
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Settings;

public static class AppSettingErrors
{
   public static readonly Error InvalidKey =
        Error.Validation("AppSetting.InvalidKey", "Key is required.");
    public static readonly Error InvalidValue =
        Error.Validation("AppSetting.InvalidValue", "القيمة مطلوبة.");

    public static readonly Error KeyAlreadyExists =
        Error.Conflict("AppSetting.KeyAlreadyExists", "مفتاح الإعدادات التطبيقية موجود بالفعل.");


}