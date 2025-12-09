using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.InjuryReasons.Commands.DeleteInjuryReason;

public class DeleteInjuryReasonCommandHandler : IRequestHandler<DeleteInjuryReasonCommand, Result<Deleted>>
{
    private readonly ILogger<DeleteInjuryReasonCommandHandler> _logger;
    private readonly IAppDbContext _context;
    private readonly HybridCache _cache;

    public DeleteInjuryReasonCommandHandler(ILogger<DeleteInjuryReasonCommandHandler> logger, IAppDbContext context, HybridCache cache)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
    }
    public async Task<Result<Deleted>> Handle(DeleteInjuryReasonCommand command, CancellationToken ct)
    {
        var injuryReason = await _context.InjuryReasons.FirstOrDefaultAsync(inj => inj.Id == command.InjuryReasonId, ct);
        if(injuryReason is null)
        {
            _logger.LogWarning("Injury reason with ID {InjuryReasonId} not found.", command.InjuryReasonId);
            return Error.NotFound(code: "InjuryReason.NotFound", description: "Injury reason not found.");
        }

        _context.InjuryReasons.Remove(injuryReason);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Injury reason with ID {InjuryReasonId} deleted successfully.", command.InjuryReasonId);

        await _cache.RemoveByTagAsync("injury-reason", ct);
        

        return Result.Deleted;
    }
}