using AlatrafClinic.Application.Features.Inventory.Stores.Dtos;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Queries.GetStoreItemUnitsQuery;

public class GetStoreItemUnitsQueryHandler : IRequestHandler<GetStoreItemUnitsQuery, Result<List<StoreItemUnitDto>>>
{
    private readonly ILogger<GetStoreItemUnitsQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetStoreItemUnitsQueryHandler(ILogger<GetStoreItemUnitsQueryHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<StoreItemUnitDto>>> Handle(GetStoreItemUnitsQuery request, CancellationToken ct)
    {
        var items = await _unitOfWork.Stores.GetItemUnitsAsync(request.StoreId, ct);

        if (items is null || !items.Any())
        {
            _logger.LogInformation("No store items found for StoreId {StoreId}", request.StoreId);
            return new List<StoreItemUnitDto>();
        }

        return items.ToList();
    }
}
