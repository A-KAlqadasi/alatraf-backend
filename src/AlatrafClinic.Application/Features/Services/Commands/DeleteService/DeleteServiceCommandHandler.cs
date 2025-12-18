
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Services.Commands.DeleteService;

public class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand, Result<Deleted>>
{
    private readonly ILogger<DeleteServiceCommandHandler> _logger;
    private readonly IAppDbContext _context;
    private readonly HybridCache _cache;

    public DeleteServiceCommandHandler(ILogger<DeleteServiceCommandHandler> logger, IAppDbContext context, HybridCache cache)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
    }
    public async Task<Result<Deleted>> Handle(DeleteServiceCommand command, CancellationToken ct)
    {
        var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == command.ServiceId, ct);

        if (service is null)
        {
            _logger.LogError("Service with id {Id} not found", command.ServiceId);
            return ServiceErrors.ServiceNotFound;
        }

        _context.Services.Remove(service);
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("service", ct);

        _logger.LogInformation("Service with id {Id} deleted", command.ServiceId);
        
        return Result.Deleted;
    }
}