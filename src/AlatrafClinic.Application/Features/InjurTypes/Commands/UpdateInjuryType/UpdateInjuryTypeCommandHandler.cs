using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.InjurTypes.Commands.UpdateInjuryType;

public class UpdateInjuryTypeCommandHandler : IRequestHandler<UpdateInjuryTypeCommand, Result<Updated>>
{
    private readonly ILogger<UpdateInjuryTypeCommandHandler> _logger;
    private readonly IAppDbContext _context;
    private readonly HybridCache _cache;

    public UpdateInjuryTypeCommandHandler(ILogger<UpdateInjuryTypeCommandHandler> logger, IAppDbContext context, HybridCache cache)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
    }
    public async Task<Result<Updated>> Handle(UpdateInjuryTypeCommand command, CancellationToken ct)
    {
        var injuryType = await _context.InjuryTypes.FirstOrDefaultAsync(inj => inj.Id == command.InjuryTypeId, ct);
        if(injuryType is null)
        {
            _logger.LogWarning("Injury type with ID {InjuryTypeId} not found.", command.InjuryTypeId);
            return Error.NotFound(code: "InjuryType.NotFound", description: "Injury type not found.");
        }

        var isExists = await _context.InjuryTypes
            .AnyAsync(x => x.Name.ToLower() == command.Name.ToLower(), ct);
        if (isExists && !string.Equals(injuryType.Name, command.Name, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Injury type with name {InjuryTypeName} already exists.", command.Name);
            return Error.Conflict(code: "InjuryType.AlreadyExists", description: "Injury type with the same name already exists.");
        }

        var injuryTypeResult = injuryType.Update(command.Name);

        if(injuryTypeResult.IsError)
        {
            _logger.LogError("Failed to upudate injury type with name {InjuryTypeName}. Error: {Error}", command.Name, injuryTypeResult.TopError);
            return Error.Failure(code: "InjuryType.UpdateFailed", description: "Failed to update injury type.");
        }

        _context.InjuryTypes.Update(injuryType);
        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("Injury type with ID {InjuryTypeId} updated successfully.", command.InjuryTypeId);
        await _cache.RemoveByTagAsync("injury-type", ct);

        return Result.Updated;
    }
}