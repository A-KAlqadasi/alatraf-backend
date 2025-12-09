using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.InjurySides.Commands.UpdateInjurySide;

public class UpdateInjurySideCommandHandler : IRequestHandler<UpdateInjurySideCommand, Result<Updated>>
{
    private readonly ILogger<UpdateInjurySideCommandHandler> _logger;
    private readonly IAppDbContext _context;
    private readonly HybridCache _cache;

    public UpdateInjurySideCommandHandler(ILogger<UpdateInjurySideCommandHandler> logger, IAppDbContext context, HybridCache cache)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
    }
    public async Task<Result<Updated>> Handle(UpdateInjurySideCommand command, CancellationToken ct)
    {
        var injurySide = await _context.InjurySides.FirstOrDefaultAsync(inj => inj.Id == command.InjurySideId, ct);
        if(injurySide is null)
        {
            _logger.LogWarning("Injury side with ID {InjurySideId} not found.", command.InjurySideId);
            return Error.NotFound(code: "InjurySide.NotFound", description: "Injury side not found.");
        }

        var isExists = await _context.InjurySides
            .AnyAsync(x => x.Name.ToLower() == command.Name.ToLower(), ct);
        if (isExists && !string.Equals(injurySide.Name, command.Name, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Injury side with name {InjurySideName} already exists.", command.Name);
            return Error.Conflict(code: "InjurySide.AlreadyExists", description: "Injury side with the same name already exists.");
        }

        var injurySideResult = injurySide.Update(command.Name);

        if(injurySideResult.IsError)
        {
            _logger.LogError("Failed to upudate injury side with name {InjurySideName}. Error: {Error}", command.Name, injurySideResult.TopError);
            return Error.Failure(code: "InjurySide.UpdateFailed", description: "Failed to update injury side.");
        }

        _context.InjurySides.Update(injurySide);
        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("Injury side with ID {InjurySideId} updated successfully.", command.InjurySideId);
        await _cache.RemoveByTagAsync("injury-side", ct);

        return Result.Updated;
    }
}