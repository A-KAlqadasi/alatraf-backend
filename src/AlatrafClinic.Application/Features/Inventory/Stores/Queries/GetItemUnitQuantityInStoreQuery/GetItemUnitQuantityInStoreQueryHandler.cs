using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Stores.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Stores;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Queries.GetItemUnitQuantityInStoreQuery;

public class GetItemUnitQuantityInStoreQueryHandler : IRequestHandler<GetItemUnitQuantityInStoreQuery, Result<decimal>>
{
    private readonly ILogger<GetItemUnitQuantityInStoreQueryHandler> _logger;
    private readonly IAppDbContext _dbContext;

    public GetItemUnitQuantityInStoreQueryHandler(ILogger<GetItemUnitQuantityInStoreQueryHandler> logger, IAppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Result<decimal>> Handle(GetItemUnitQuantityInStoreQuery request, CancellationToken ct)
    {
        var items = await _dbContext.StoreItemUnits
            .AsNoTracking()
            .Where(siu => siu.StoreId == request.StoreId)
            .Include(siu => siu.ItemUnit)
            .ToListAsync(ct);

        if (items is null || !items.Any())
        {
            _logger.LogInformation("No items for StoreId {StoreId}", request.StoreId);
            return StoreErrors.StoreNotFound;
        }

        var match = items.FirstOrDefault(siu => siu.ItemUnit.ItemId == request.ItemId && siu.ItemUnit.UnitId == request.UnitId);
        if (match is null)
        {
            _logger.LogInformation("ItemUnit not found in store {StoreId} for ItemId {ItemId} UnitId {UnitId}", request.StoreId, request.ItemId, request.UnitId);
            return StoreItemUnitErrors.NotFound;
        }

        return match.Quantity;
    }
}
