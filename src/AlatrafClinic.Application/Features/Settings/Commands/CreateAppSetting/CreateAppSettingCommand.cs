using AlatrafClinic.Application.Features.Settings.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Settings.Enums;

using MediatR;

namespace AlatrafClinic.Application.Features.Settings.Commands;

public sealed record CreateAppSettingCommand(
    string Key,
    string Value,
    AppSettingType Type,
    string? Description
) : IRequest<Result<AppSettingDto>>;
