
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Services.Commands.UpdateService;

public class UpdateServiceCommandHandler : IRequestHandler<UpdateServiceCommand, Result<Updated>>
{
    private readonly ILogger<UpdateServiceCommandHandler> _logger;
    private readonly IAppDbContext _context;
    private readonly HybridCache _cache;

    public UpdateServiceCommandHandler(ILogger<UpdateServiceCommandHandler> logger, IAppDbContext context, HybridCache cache)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
    }
    public async Task<Result<Updated>> Handle(UpdateServiceCommand command, CancellationToken ct)
    {
        var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == command.ServiceId, ct);
        if (service is null)
        {
            _logger.LogError("Service with ID {ServiceId} not found.", command.ServiceId);
            return ServiceErrors.ServiceNotFound;
        }

        var result = service.Update(command.Name, command.DepartmentId, command.Price);
        
        if (result.IsError)
        {
            _logger.LogError("Failed to update service with ID {ServiceId}. Error: {Error}", command.ServiceId, result.TopError);

            return result.Errors;
        }
        
        _context.Services.Update(service);
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("service", ct);

        _logger.LogInformation("Service {serviceId} updated successfully", service.Id);
        
        return Result.Updated;
    }
}