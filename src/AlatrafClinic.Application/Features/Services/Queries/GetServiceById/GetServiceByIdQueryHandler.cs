
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Services.Dtos;
using AlatrafClinic.Application.Features.Services.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Services.Queries.GetServiceById;

public class GetServiceByIdQueryHandler : IRequestHandler<GetServiceByIdQuery, Result<ServiceDto>>
{
    private readonly ILogger<GetServiceByIdQueryHandler> _logger;
    private readonly IAppDbContext _context;

    public GetServiceByIdQueryHandler(ILogger<GetServiceByIdQueryHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Result<ServiceDto>> Handle(GetServiceByIdQuery query, CancellationToken cancellationToken)
    {
        var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == query.ServiceId, cancellationToken);
        if (service == null)
        {
            _logger.LogWarning("Service not found: {ServiceId}", query.ServiceId);

            return ServiceErrors.ServiceNotFound;
        }
        
        return service.ToDto();
    }
}