using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Settings.Dtos;
using AlatrafClinic.Application.Features.Settings.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Settings;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Settings.Commands;

public sealed class CreateAppSettingCommandHandler(
IAppDbContext _context,
ILogger<CreateAppSettingCommandHandler> _logger
)
    : IRequestHandler<CreateAppSettingCommand, Result<AppSettingDto>>
{
    public async Task<Result<AppSettingDto>> Handle(CreateAppSettingCommand command, CancellationToken ct)
    {
        var existing = await _context.AppSettings.FirstOrDefaultAsync(a=> a.Key == command.Key, ct);
        
        if (existing is not null)
        {
            _logger.LogError("App setting with key {Key} already exists", command.Key);
            return AppSettingErrors.KeyAlreadyExists;
        }

        var result = AppSetting.Create(
            command.Key,
            command.Value,
            command.Description
        );

        if (result.IsError)
        {
            return result.Errors;
        }

        var newAppSetting = result.Value;

        await _context.AppSettings.AddAsync(newAppSetting, ct);
        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("App setting with key {Key} created successfully", command.Key);

        return newAppSetting.ToDto();
    }
}