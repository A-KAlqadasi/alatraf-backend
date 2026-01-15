using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;
using AlatrafClinic.Domain.Inventory.Stores;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Commands.AddItemUnitToStore;

public class AddItemUnitToStoreCommandHandler : IRequestHandler<AddItemUnitToStoreCommand, Result<Updated>>
{
    private readonly ILogger<AddItemUnitToStoreCommandHandler> _logger;
    private readonly IAppDbContext _dbContext;

    public AddItemUnitToStoreCommandHandler(ILogger<AddItemUnitToStoreCommandHandler> logger, IAppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Result<Updated>> Handle(AddItemUnitToStoreCommand command, CancellationToken ct)
    {
        // Load item unit (validate reference)
        var itemUnit = await _dbContext.ItemUnits.SingleOrDefaultAsync(iu => iu.ItemId == command.ItemId && iu.UnitId == command.UnitId, ct);
        if (itemUnit is null)
        {
            _logger.LogWarning("Item unit not found for ItemId={ItemId} UnitId={UnitId}", command.ItemId, command.UnitId);
            return ItemUnitErrors.ItemUnitNotFound;
        }

        // Load aggregate root Store (with its item units)
        var store = await _dbContext.Stores
            .Include(s => s.StoreItemUnits)
                .ThenInclude(siu => siu.ItemUnit)
            .SingleOrDefaultAsync(s => s.Id == command.StoreId, ct);
        if (store is null)
        {
            _logger.LogWarning("Store not found with id {StoreId}", command.StoreId);
            return StoreErrors.StoreNotFound;
        }

        // Delegate to aggregate root to enforce invariants
        var addResult = store.AddItemUnit(itemUnit, command.Quantity);
        if (addResult.IsError)
        {
            _logger.LogWarning("Failed to add item unit to store {StoreId}: {Errors}", command.StoreId, string.Join(',', addResult.Errors));
            return addResult.Errors;
        }

        //_dbContext.Stores.Update(store);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Added ItemUnit (ItemId={ItemId},UnitId={UnitId}) to Store {StoreId} qty={Qty}", command.ItemId, command.UnitId, command.StoreId, command.Quantity);

        return Result.Updated;
    }
}
