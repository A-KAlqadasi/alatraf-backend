using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.IndustrialParts.Commands.DeleteIndustrialPart;

public class DeleteIndustrialPartCommandHandler : IRequestHandler<DeleteIndustrialPartCommand, Result<Deleted>>
{
    private readonly ILogger<DeleteIndustrialPartCommandHandler> _logger;
    private readonly IAppDbContext _context;
    private readonly HybridCache _cache;

    public DeleteIndustrialPartCommandHandler(ILogger<DeleteIndustrialPartCommandHandler> logger, IAppDbContext context, HybridCache cache)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
    }
    public async Task<Result<Deleted>> Handle(DeleteIndustrialPartCommand command, CancellationToken ct)
    {
        var industrialPart = await _context.IndustrialParts.FirstOrDefaultAsync(i=> i.Id == command.IndustrialPartId, ct);
        if (industrialPart is null)
        {
            _logger.LogError("Industrial Part with Id {IndustrialPartId} not found.", command.IndustrialPartId);
            return Result.Deleted;
        }

        _context.IndustrialParts.Remove(industrialPart);
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("industrial-part");

        _logger.LogInformation("Industrial Part with Id {IndustrialPartId} deleted successfully.", command.IndustrialPartId);
        
        return Result.Deleted;
    }
}