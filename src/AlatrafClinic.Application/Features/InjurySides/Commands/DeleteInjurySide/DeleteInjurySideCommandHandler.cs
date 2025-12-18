using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.InjurySides.Commands.DeleteInjurySide;

public class DeleteInjurySideCommandHandler : IRequestHandler<DeleteInjurySideCommand, Result<Deleted>>
{
    private readonly ILogger<DeleteInjurySideCommandHandler> _logger;
    private readonly IAppDbContext _context;
    private readonly HybridCache _cache;

    public DeleteInjurySideCommandHandler(ILogger<DeleteInjurySideCommandHandler> logger, IAppDbContext context, HybridCache cache)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
    }
    public async Task<Result<Deleted>> Handle(DeleteInjurySideCommand command, CancellationToken ct)
    {
        var injurySide = await _context.InjurySides.FirstOrDefaultAsync(inj => inj.Id == command.InjurySideId, ct);
        if(injurySide is null)
        {
            _logger.LogWarning("Injury side with ID {InjurySideId} not found.", command.InjurySideId);
            return Error.NotFound(code: "InjurySide.NotFound", description: "Injury side not found.");
        }

        _context.InjurySides.Remove(injurySide);
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("injury-side", ct);

        _logger.LogInformation("Injury side with ID {InjurySideId} deleted successfully.", command.InjurySideId);

        return Result.Deleted;
    }
}