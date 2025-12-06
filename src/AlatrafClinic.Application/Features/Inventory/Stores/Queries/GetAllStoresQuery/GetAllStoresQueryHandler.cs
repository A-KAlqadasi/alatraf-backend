using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.Stores.Dtos;
using AlatrafClinic.Application.Features.Inventory.Stores.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Queries.GetAllStoresQuery;

public class GetAllStoresQueryHandler : IRequestHandler<GetAllStoresQuery, Result<List<StoreDto>>>
{
    private readonly ILogger<GetAllStoresQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetAllStoresQueryHandler(ILogger<GetAllStoresQueryHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<StoreDto>>> Handle(GetAllStoresQuery request, CancellationToken ct)
    {
        var stores = await _unitOfWork.Stores.GetAllAsync(ct);
        return stores.ToDtos();
    }
}
