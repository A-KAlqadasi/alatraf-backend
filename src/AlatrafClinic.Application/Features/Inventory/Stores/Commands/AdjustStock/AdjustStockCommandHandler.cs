using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Stores;
using AlatrafClinic.Domain.Inventory.Items;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Commands.AdjustStock;

public class AdjustStockCommandHandler : IRequestHandler<AdjustStockCommand, Result<Updated>>
{
    private readonly ILogger<AdjustStockCommandHandler> _logger;
    private readonly IAppDbContext _dbContext;

    public AdjustStockCommandHandler(ILogger<AdjustStockCommandHandler> logger, IAppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Result<Updated>> Handle(AdjustStockCommand command, CancellationToken ct)
    {
        if (command.Quantity <= 0)
        {
            return StoreItemUnitErrors.InvalidQuantity;
        }

        var itemUnit = await _dbContext.ItemUnits.SingleOrDefaultAsync(iu => iu.ItemId == command.ItemId && iu.UnitId == command.UnitId, ct);
        if (itemUnit is null)
        {
            _logger.LogWarning("ItemUnit not found for Item {ItemId} and Unit {UnitId}", command.ItemId, command.UnitId);
            return ItemUnitErrors.ItemUnitNotFound;
        }

        var store = await _dbContext.Stores
            .Include(s => s.StoreItemUnits)
                .ThenInclude(siu => siu.ItemUnit)
            .SingleOrDefaultAsync(s => s.Id == command.StoreId, ct);
        if (store is null)
        {
            _logger.LogWarning("Store {StoreId} not found when adjusting stock.", command.StoreId);
            return StoreErrors.StoreNotFound;
        }

        var quantityDelta = command.Increase ? command.Quantity : -command.Quantity;

        var adjustResult = store.AdjustItemUnit(itemUnit, quantityDelta);
        if (adjustResult.IsError)
        {
            _logger.LogWarning("Failed to adjust stock for Store {StoreId}: {Errors}", command.StoreId, string.Join(',', adjustResult.Errors));
            return adjustResult.Errors;
        }

        _dbContext.Stores.Update(store);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Adjusted stock for Store {StoreId} ItemUnit {ItemUnitId} by {Delta}", command.StoreId, itemUnit.Id, quantityDelta);

        return Result.Updated;
    }
}
