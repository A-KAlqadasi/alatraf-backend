using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.Stores.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Stores;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Queries.GetItemUnitQuantityInStoreQuery;

public class GetItemUnitQuantityInStoreQueryHandler : IRequestHandler<GetItemUnitQuantityInStoreQuery, Result<decimal>>
{
    private readonly ILogger<GetItemUnitQuantityInStoreQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetItemUnitQuantityInStoreQueryHandler(ILogger<GetItemUnitQuantityInStoreQueryHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<decimal>> Handle(GetItemUnitQuantityInStoreQuery request, CancellationToken ct)
    {
        var items = await _unitOfWork.Stores.GetItemUnitsAsync(request.StoreId, ct);

        if (items is null || !items.Any())
        {
            _logger.LogInformation("No items for StoreId {StoreId}", request.StoreId);
            return StoreErrors.StoreNotFound;
        }

        var found = items.FirstOrDefault(i => i.ItemId == request.ItemId && i.UnitId == request.UnitId);

        if (found is null)
        {
            _logger.LogInformation("ItemUnit not found in store {StoreId} for ItemId {ItemId} UnitId {UnitId}", request.StoreId, request.ItemId, request.UnitId);
            return StoreItemUnitErrors.NotFound;
        }

        return found.Quantity;
    }
}
