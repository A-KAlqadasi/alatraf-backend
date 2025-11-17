using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AlatrafClinic.Domain.Settings.Enums;

namespace AlatrafClinic.Application.Features.Settings.Dtos;


public sealed class AppSettingDto
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AppSettingType Type { get; set; }
}