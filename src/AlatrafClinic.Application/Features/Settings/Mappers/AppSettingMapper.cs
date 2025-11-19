using AlatrafClinic.Application.Features.Settings.Dtos;
using AlatrafClinic.Domain.Settings;

namespace AlatrafClinic.Application.Features.Settings.Mappers;

public static class AppSettingMapper
{
    public static AppSettingDto ToDto(this AppSetting setting)
    {
        return new AppSettingDto
        {
            Key = setting.Key,
            Value = setting.Value,
            Description = setting.Description,
            Type = setting.Type
        };
    }

    public static List<AppSettingDto> ToDtos(this IEnumerable<AppSetting> settings)
    {
        return settings.Select(s => s.ToDto()).ToList();
    }
}