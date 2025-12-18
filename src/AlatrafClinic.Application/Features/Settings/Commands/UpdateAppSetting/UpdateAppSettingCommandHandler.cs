using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Settings.Commands.UpdateAppSetting;

public sealed class UpdateAppSettingCommandHandler(IAppDbContext _context, ILogger<UpdateAppSettingCommandHandler> _logger)
    : IRequestHandler<UpdateAppSettingCommand, Result<Updated>>
{

    public async Task<Result<Updated>> Handle(UpdateAppSettingCommand command, CancellationToken ct)
    {
        var setting = await _context.AppSettings.FirstOrDefaultAsync(a=> a.Key == command.Key, ct);
        if (setting is null)
        {
            _logger.LogError("App setting with key {Key} not found", command.Key);
            return ApplicationErrors.AppSettingKeyNotFound;
        }

        var result = setting.Update(command.Value, command.Description);
        if (result.IsError)
        {
            return result.Errors;
        }


        _context.AppSettings.Update(setting);
        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("App setting with key {Key} updated successfully", command.Key);

        return Result.Updated;
    }
}