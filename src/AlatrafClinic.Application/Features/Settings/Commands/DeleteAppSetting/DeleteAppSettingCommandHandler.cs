using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Settings.Commands.DeleteAppSetting;

public sealed class DeleteAppSettingCommandHandler(IAppDbContext _context, ILogger<DeleteAppSettingCommandHandler> _logger)
    : IRequestHandler<DeleteAppSettingCommand, Result<Deleted>>
{

    public async Task<Result<Deleted>> Handle(DeleteAppSettingCommand command, CancellationToken ct)
    {
        var setting = await _context.AppSettings.FirstOrDefaultAsync(a=> a.Key == command.Key, ct);
        if (setting is null)
        {
            _logger.LogError("App setting with key {Key} not found", command.Key);
            return ApplicationErrors.AppSettingKeyNotFound;
        }

        _context.AppSettings.Remove(setting);
        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("App setting with key {Key} deleted successfully", command.Key);

        return Result.Deleted;
    }
}