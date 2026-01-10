using AlatrafClinic.Application.Features.Inventory.Stores.Dtos;
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Queries.GetStoreItemUnitsQuery;

public class GetStoreItemUnitsQueryHandler : IRequestHandler<GetStoreItemUnitsQuery, Result<List<StoreItemUnitDto>>>
{
    private readonly ILogger<GetStoreItemUnitsQueryHandler> _logger;
    private readonly IAppDbContext _dbContext;

    public GetStoreItemUnitsQueryHandler(ILogger<GetStoreItemUnitsQueryHandler> logger, IAppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Result<List<StoreItemUnitDto>>> Handle(GetStoreItemUnitsQuery request, CancellationToken ct)
    {
        var items = await _dbContext.StoreItemUnits
            .AsNoTracking()
            .Where(siu => siu.StoreId == request.StoreId)
            .Include(siu => siu.ItemUnit)
                .ThenInclude(iu => iu.Item)
            .Include(siu => siu.ItemUnit)
                .ThenInclude(iu => iu.Unit)
            .Select(siu => new StoreItemUnitDto
            {
                StoreItemUnitId = siu.Id,
                StoreId = siu.StoreId,
                ItemId = siu.ItemUnit.ItemId,
                ItemName = siu.ItemUnit.Item.Name,
                UnitId = siu.ItemUnit.UnitId,
                UnitName = siu.ItemUnit.Unit.Name,
                Quantity = siu.Quantity
            })
            .ToListAsync(ct);

        if (items is null || !items.Any())
        {
            _logger.LogInformation("No store items found for StoreId {StoreId}", request.StoreId);
            return new List<StoreItemUnitDto>();
        }

        return items.ToList();
    }
}
