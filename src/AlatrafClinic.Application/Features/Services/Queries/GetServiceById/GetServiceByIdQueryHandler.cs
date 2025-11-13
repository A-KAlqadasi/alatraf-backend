
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
    private readonly IUnitOfWork _unitOfWork;

    public GetServiceByIdQueryHandler(ILogger<GetServiceByIdQueryHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ServiceDto>> Handle(GetServiceByIdQuery query, CancellationToken cancellationToken)
    {
        var service = await _unitOfWork.Services.GetByIdAsync(query.ServiceId, cancellationToken);
        if (service == null)
        {
            _logger.LogWarning("Service not found: {ServiceId}", query.ServiceId);

            return ServiceErrors.ServiceNotFound;
        }
        
        return service.ToDto();
    }
}