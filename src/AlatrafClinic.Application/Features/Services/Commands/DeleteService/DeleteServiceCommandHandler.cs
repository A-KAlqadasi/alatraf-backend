
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services;

using MediatR;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Services.Commands.DeleteService;

public class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand, Result<Deleted>>
{
    private readonly ILogger<DeleteServiceCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly HybridCache _cache;

    public DeleteServiceCommandHandler(ILogger<DeleteServiceCommandHandler> logger, IUnitOfWork unitOfWork, HybridCache cache)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }
    public async Task<Result<Deleted>> Handle(DeleteServiceCommand command, CancellationToken ct)
    {
        var service = await _unitOfWork.Services.GetByIdAsync(command.ServiceId, ct);

        if (service is null)
        {
            _logger.LogWarning("Service with id {Id} not found", command.ServiceId);
            return ServiceErrors.ServiceNotFound;
        }

        if(await _unitOfWork.Services.HasAssociationsAsync(command.ServiceId, ct))
        {
            _logger.LogWarning("Service with id {Id} has associated records and cannot be deleted", command.ServiceId);
            return Error.Conflict(code: "Service.HasAssociations", description: $"Service with Id {command.ServiceId} has associations and cannot be deleted.");
        }

        await _unitOfWork.Services.DeleteAsync(service);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Service with id {Id} deleted", command.ServiceId);
        
        return Result.Deleted;
    }
}