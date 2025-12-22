
using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Settings.Enums;

namespace AlatrafClinic.Domain.Settings;

public sealed class AppSetting : AuditableEntity<int>
{
    public string Key { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public AppSettingType Type { get; private set; }
    public string? Description { get; private set; }

    private AppSetting() { }
    private AppSetting(string key, string value, AppSettingType type, string? description)
    {
        Key = key;
        Value = value;
        Type = type;
        Description = description;
    }
    public static Result<AppSetting> Create(string key, string value, AppSettingType type, string? description)
    {
        if (string.IsNullOrWhiteSpace(key))
            return AppSettingErrors.InvalidKey;

        if (!IsValidType(type))
            return AppSettingErrors.InvalidType;

        if (!IsValidValue(type, value))
            return AppSettingErrors.InvalidValue;

        return new AppSetting(key.Trim(), value.Trim(), type, description);
    }

    public Result<Updated> Update(string value, string? description)
    {
        if (!IsValidValue(Type, value))
            return AppSettingErrors.InvalidValue;

        Value = value.Trim();
        Description = description;

        return Result.Updated;
    }
    private static bool IsValidType(AppSettingType type)
    {
        return Enum.IsDefined(typeof(AppSettingType), type);
    }
     private static bool IsValidValue(AppSettingType type, string value)
    {
        try
        {
            return type switch
            {
                AppSettingType.String => true,
                AppSettingType.Integer => int.TryParse(value, out _),
                AppSettingType.Boolean => bool.TryParse(value, out _),
                AppSettingType.Decimal => decimal.TryParse(value, out _),
                AppSettingType.Json => IsValidJson(value),
                _ => false
            };
        }
        catch
        {
            return false;
        }
    }

    private static bool IsValidJson(string value)
    {
        try
        {
            System.Text.Json.JsonDocument.Parse(value);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
}
