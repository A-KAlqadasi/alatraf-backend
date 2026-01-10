using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;
using AlatrafClinic.Domain.Inventory.Stores;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Commands.TransferStock;

public class TransferStockCommandHandler : IRequestHandler<TransferStockCommand, Result<Updated>>
{
    private readonly ILogger<TransferStockCommandHandler> _logger;
    private readonly IAppDbContext _dbContext;

    public TransferStockCommandHandler(ILogger<TransferStockCommandHandler> logger, IAppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Result<Updated>> Handle(TransferStockCommand command, CancellationToken ct)
    {
        if (command.Quantity <= 0)
        {
            return StoreItemUnitErrors.InvalidQuantity;
        }

        if (command.SourceStoreId == command.DestinationStoreId)
        {
            _logger.LogWarning("Transfer between same store {StoreId} attempted", command.SourceStoreId);
            return Error.Validation("Store.Transfer.SameStore", "Source and destination stores must be different.");
        }

        // validate item unit exists
        var itemUnit = await _dbContext.ItemUnits.SingleOrDefaultAsync(iu => iu.ItemId == command.ItemId && iu.UnitId == command.UnitId, ct);
        if (itemUnit is null)
        {
            _logger.LogWarning("ItemUnit not found for ItemId={ItemId} UnitId={UnitId}", command.ItemId, command.UnitId);
            return ItemUnitErrors.ItemUnitNotFound;
        }

        // load source and destination aggregate roots
        var sourceStore = await _dbContext.Stores
            .Include(s => s.StoreItemUnits)
                .ThenInclude(siu => siu.ItemUnit)
            .SingleOrDefaultAsync(s => s.Id == command.SourceStoreId, ct);
        if (sourceStore is null)
        {
            _logger.LogWarning("Source store not found: {StoreId}", command.SourceStoreId);
            return StoreErrors.StoreNotFound;
        }

        var destStore = await _dbContext.Stores
            .Include(s => s.StoreItemUnits)
                .ThenInclude(siu => siu.ItemUnit)
            .SingleOrDefaultAsync(s => s.Id == command.DestinationStoreId, ct);
        if (destStore is null)
        {
            _logger.LogWarning("Destination store not found: {StoreId}", command.DestinationStoreId);
            return StoreErrors.StoreNotFound;
        }

        // decrease from source
        var decreaseResult = sourceStore.AdjustItemUnit(itemUnit, -command.Quantity);
        if (decreaseResult.IsError)
        {
            _logger.LogWarning("Failed to decrease stock in source store {StoreId}: {Errors}", command.SourceStoreId, string.Join(',', decreaseResult.Errors));
            return decreaseResult.Errors;
        }

        // increase in destination
        var increaseResult = destStore.AdjustItemUnit(itemUnit, command.Quantity);
        if (increaseResult.IsError)
        {
            _logger.LogWarning("Failed to increase stock in destination store {StoreId}: {Errors}. Attempting to revert source.", command.DestinationStoreId, string.Join(',', increaseResult.Errors));

            // attempt to revert source decrease in-memory
            var revert = sourceStore.AdjustItemUnit(itemUnit, command.Quantity);
            if (revert.IsError)
            {
                _logger.LogError("Failed to revert source store after failed transfer: {Errors}", string.Join(',', revert.Errors));
                return Error.Conflict("Store.Transfer.FailedRevert", "Transfer failed and revert could not be applied. Manual reconciliation required.");
            }

            return increaseResult.Errors;
        }

        // persist both aggregates
        _dbContext.Stores.Update(sourceStore);
        _dbContext.Stores.Update(destStore);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Transferred {Qty} of ItemId={ItemId} UnitId={UnitId} from Store {Source} to Store {Dest}", command.Quantity, command.ItemId, command.UnitId, command.SourceStoreId, command.DestinationStoreId);

        return Result.Updated;
    }
}
