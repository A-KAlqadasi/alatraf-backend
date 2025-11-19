

using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Settings.Commands.UpdateAppSetting;

public sealed record UpdateAppSettingCommand(
    string Key,
    string Value,
    string? Description
) : IRequest<Result<Updated>>;
