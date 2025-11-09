
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services;

using MediatR;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Services.Commands.UpdateService;

public class UpdateServiceCommandHandler : IRequestHandler<UpdateServiceCommand, Result<Updated>>
{
    private readonly ILogger<UpdateServiceCommandHandler> _logger;
    private readonly IUnitOfWork _uow;
    private readonly HybridCache _cache;

    public UpdateServiceCommandHandler(ILogger<UpdateServiceCommandHandler> logger, IUnitOfWork uow, HybridCache cache)
    {
        _logger = logger;
        _uow = uow;
        _cache = cache;
    }
    public async Task<Result<Updated>> Handle(UpdateServiceCommand command, CancellationToken ct)
    {
        var service = await _uow.Services.GetByIdAsync(command.ServiceId, ct);
        if (service is null)
        {
            _logger.LogWarning("Service with ID {ServiceId} not found.", command.ServiceId);
            return ServiceErrors.ServiceNotFound;
        }

        var result = service.Update(command.Name, command.DepartmentId, command.Price);
        if (result.IsError)
        {
            _logger.LogWarning("Failed to update service with ID {ServiceId}. Error: {Error}", command.ServiceId, result.TopError);

            return result.Errors;
        }

        await _uow.Services.UpdateAsync(service, ct);
        await _uow.SaveChangesAsync(ct);
        
        return Result.Updated;
    }
}