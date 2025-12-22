using AlatrafClinic.Application.Features.Settings.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Settings.Commands;

public sealed record CreateAppSettingCommand(
    string Key,
    string Value,
    string? Description
) : IRequest<Result<AppSettingDto>>;
