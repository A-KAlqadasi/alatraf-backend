using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.InjuryReasons.Commands.UpdateInjuryReason;

public class UpdateInjuryReasonCommandHandler : IRequestHandler<UpdateInjuryReasonCommand, Result<Updated>>
{
    private readonly ILogger<UpdateInjuryReasonCommandHandler> _logger;
    private readonly IAppDbContext _context;
    private readonly HybridCache _cache;

    public UpdateInjuryReasonCommandHandler(ILogger<UpdateInjuryReasonCommandHandler> logger, IAppDbContext context, HybridCache cache)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
    }
    public async Task<Result<Updated>> Handle(UpdateInjuryReasonCommand command, CancellationToken ct)
    {
        var injuryReason = await _context.InjuryReasons.FirstOrDefaultAsync(inj => inj.Id == command.InjuryReasonId, ct);
        if(injuryReason is null)
        {
            _logger.LogWarning("Injury reason with ID {InjuryReasonId} not found.", command.InjuryReasonId);
            return Error.NotFound(code: "InjuryReason.NotFound", description: "Injury reason not found.");
        }

        var isExists = await _context.InjuryReasons
            .AnyAsync(x => x.Name.ToLower() == command.Name.ToLower(), ct);
        if (isExists && !string.Equals(injuryReason.Name, command.Name, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Injury reason with name {InjuryReasonName} already exists.", command.Name);
            return Error.Conflict(code: "InjuryReason.AlreadyExists", description: "Injury reason with the same name already exists.");
        }

        var injuryReasonResult = injuryReason.Update(command.Name);

        if(injuryReasonResult.IsError)
        {
            _logger.LogError("Failed to upudate injury reason with name {InjuryReasonName}. Error: {Error}", command.Name, injuryReasonResult.TopError);
            return Error.Failure(code: "InjuryReason.UpdateFailed", description: "Failed to update injury reason.");
        }

        _context.InjuryReasons.Update(injuryReason);
        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("Injury reason with ID {InjuryReasonId} updated successfully.", command.InjuryReasonId);

        await _cache.RemoveByTagAsync("injury-reason", ct);

        return Result.Updated;
    }
}