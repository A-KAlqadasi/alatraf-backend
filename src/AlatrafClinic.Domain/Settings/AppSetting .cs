
using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Settings;

public sealed class AppSetting : AuditableEntity<int>
{
    public string Key { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    private AppSetting() { }
    private AppSetting(string key, string value, string? description)
    {
        Key = key;
        Value = value;
        Description = description;
    }
    public static Result<AppSetting> Create(string key, string value, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            return AppSettingErrors.InvalidKey;
        
        if (string.IsNullOrWhiteSpace(value))
            return AppSettingErrors.InvalidValue;

       
        return new AppSetting(key.Trim(), value.Trim(), description);
    }

    public Result<Updated> Update(string value, string? description)
    {
       
        Value = value.Trim();
        Description = description;

        return Result.Updated;
    }
}
