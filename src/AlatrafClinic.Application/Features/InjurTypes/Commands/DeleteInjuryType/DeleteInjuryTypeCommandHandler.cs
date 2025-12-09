using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.InjurTypes.Commands.DeleteInjuryType;

public class DeleteInjuryTypeCommandHandler : IRequestHandler<DeleteInjuryTypeCommand, Result<Deleted>>
{
    private readonly ILogger<DeleteInjuryTypeCommandHandler> _logger;
    private readonly IAppDbContext _context;
    private readonly HybridCache _cache;

    public DeleteInjuryTypeCommandHandler(ILogger<DeleteInjuryTypeCommandHandler> logger, IAppDbContext context, HybridCache cache)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
    }
    public async Task<Result<Deleted>> Handle(DeleteInjuryTypeCommand command, CancellationToken ct)
    {
        var injuryType = await _context.InjuryTypes.FirstOrDefaultAsync(inj => inj.Id == command.InjuryTypeId, ct);
        if(injuryType is null)
        {
            _logger.LogWarning("Injury type with ID {InjuryTypeId} not found.", command.InjuryTypeId);
            return Error.NotFound(code: "InjuryType.NotFound", description: "Injury type not found.");
        }

        _context.InjuryTypes.Remove(injuryType);
        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("Injury type with ID {InjuryTypeId} deleted successfully.", command.InjuryTypeId);
        await _cache.RemoveByTagAsync("injury-type", ct);

        return Result.Deleted;
    }
}