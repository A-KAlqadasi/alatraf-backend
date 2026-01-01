using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Settings.Commands.DeleteAppSetting;

public sealed record DeleteAppSettingCommand(
    string Key
) : IRequest<Result<Deleted>>;