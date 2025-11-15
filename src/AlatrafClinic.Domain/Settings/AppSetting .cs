
using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Settings;

public sealed class AppSetting : AuditableEntity<int>
{
    public string Key { get; private set; }
    public string Value { get; private set; }
    public string? Type { get; private set; } // optional: bool, int, date, json
    public string? Description { get; private set; }


    // private AppSetting() { }

    private AppSetting(string key, string value, string? type = null, string? description = null)
    {
        Key = key.Trim();
        Value = value.Trim();
        Type = type?.Trim();
        Description = description?.Trim();
    }
    public static Result<AppSetting> Create(string key, string value, string? type = null, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            return AppSettingErrors.KeyRequired;

        if (value is null)
            return AppSettingErrors.ValueRequired;

        if (!string.IsNullOrWhiteSpace(type))
        {
            switch (type.ToLower())
            {
                case "bool":
                    if (!bool.TryParse(value, out _))
                        return AppSettingErrors.InvalidBoolValue;
                    break;

                case "int":
                    if (!int.TryParse(value, out _))
                        return AppSettingErrors.InvalidIntValue;
                    break;
                case "decimal":
                    if (!decimal.TryParse(value, out _))
                        return AppSettingErrors.InvalidDecimalValue;
                    break;

                case "date":
                    if (!DateTime.TryParse(value, out _))
                        return AppSettingErrors.InvalidDateValue;
                    break;


                default:
                    // unknown type, allow any string
                    break;
            }
        }

        return new AppSetting(key, value, type, description);
    }
    public Result<Updated> UpdateValue(string newValue)
    {
        if (string.IsNullOrWhiteSpace(newValue))
            return AppSettingErrors.ValueRequired;

        newValue = newValue.Trim();

        if (!string.IsNullOrWhiteSpace(Type))
        {
            switch (Type.ToLower())
            {
                case "bool":
                    if (!bool.TryParse(newValue, out _))
                        return AppSettingErrors.InvalidBoolValue;
                    break;

                case "int":
                    if (!int.TryParse(newValue, out _))
                        return AppSettingErrors.InvalidIntValue;
                    break;

                case "date":
                    if (!DateTime.TryParse(newValue, out _))
                        return AppSettingErrors.InvalidDateValue;
                    break;



                default:
                    break;
            }
        }

        Value = newValue;
        return Result.Updated;
    }

}
