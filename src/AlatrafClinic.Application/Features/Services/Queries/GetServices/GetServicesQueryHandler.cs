using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Services.Dtos;
using AlatrafClinic.Application.Features.Services.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Services.Queries.GetServices;

public class GetServicesQueryHandler : IRequestHandler<GetServicesQuery, Result<List<ServiceDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetServicesQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }
    public async Task<Result<List<ServiceDto>>> Handle(GetServicesQuery query, CancellationToken ct)
    {
        var services = await _uow.Services.GetAllAsync(ct);
        
        return services.ToDtos();
    }
}