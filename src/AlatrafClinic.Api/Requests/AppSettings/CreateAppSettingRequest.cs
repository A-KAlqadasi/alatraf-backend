using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.AppSettings;

public sealed class CreateAppSettingRequest
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Key { get; init; } = string.Empty;

    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string Value { get; init; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; init; }
}

public sealed class UpdateAppSettingRequest
{
    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string Value { get; init; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; init; }
}
