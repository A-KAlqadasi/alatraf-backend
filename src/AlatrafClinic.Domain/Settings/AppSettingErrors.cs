
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Settings;

public static class AppSettingErrors
{
   public static readonly Error InvalidKey =
        Error.Validation("AppSetting.InvalidKey", "Key is required.");
    public static readonly Error InvalidValue =
        Error.Validation("AppSetting.InvalidValue", "Value is not valid for the specified type.");

    public static readonly Error KeyAlreadyExists =
        Error.Conflict("AppSetting.KeyAlreadyExists", "An AppSetting with the same key already exists.");



}