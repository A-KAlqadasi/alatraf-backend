namespace AlatrafClinic.Application.Features.Settings.Dtos;

public sealed class AppSettingDto
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
}