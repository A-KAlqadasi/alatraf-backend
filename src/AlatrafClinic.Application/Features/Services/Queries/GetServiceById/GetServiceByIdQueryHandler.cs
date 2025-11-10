
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Services.Dtos;
using AlatrafClinic.Application.Features.Services.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Services.Queries.GetServiceById;

public class GetServiceByIdQueryHandler : IRequestHandler<GetServiceByIdQuery, Result<ServiceDto>>
{
    private readonly ILogger<GetServiceByIdQueryHandler> _logger;
    private readonly IUnitOfWork _uow;

    public GetServiceByIdQueryHandler(ILogger<GetServiceByIdQueryHandler> logger, IUnitOfWork uow)
    {
        _logger = logger;
        _uow = uow;
    }

    public async Task<Result<ServiceDto>> Handle(GetServiceByIdQuery query, CancellationToken cancellationToken)
    {
        var service = await _uow.Services.GetByIdAsync(query.ServiceId, cancellationToken);
        if (service == null)
        {
            _logger.LogWarning("Service not found: {ServiceId}", query.ServiceId);

            return ServiceErrors.ServiceNotFound;
        }
        
        return service.ToDto();
    }
}