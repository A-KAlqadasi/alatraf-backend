
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Settings;

public static class AppSettingErrors
{
  public static readonly Error KeyRequired =
      Error.Validation("AppSetting.KeyRequired", "The setting key is required.");

  public static readonly Error ValueRequired =
      Error.Validation("AppSetting.ValueRequired", "The setting value is required.");

  public static readonly Error InvalidBoolValue =
      Error.Validation("AppSetting.InvalidBoolValue", "The setting value must be a valid boolean.");

  public static readonly Error InvalidIntValue =
      Error.Validation("AppSetting.InvalidIntValue", "The setting value must be a valid integer.");

  public static readonly Error InvalidDateValue =
      Error.Validation("AppSetting.InvalidDateValue", "The setting value must be a valid date.");

  public static readonly Error InvalidJsonValue =
      Error.Validation("AppSetting.InvalidJsonValue", "The setting value must be valid JSON.");
 public static readonly Error InvalidDecimalValue =
      Error.Validation("AppSetting.InvalidDecimalValue", "The setting value must be valid decimal.");

  public static readonly Error DuplicateKey =
      Error.Conflict("AppSetting.DuplicateKey", "A setting with this key already exists.");

  public static readonly Error CannotDeleteSetting =
      Error.Conflict("AppSetting.CannotDelete", "This setting cannot be deleted due to existing dependencies.");
}