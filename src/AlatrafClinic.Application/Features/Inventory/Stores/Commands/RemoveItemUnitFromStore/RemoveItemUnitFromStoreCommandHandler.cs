using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;
using AlatrafClinic.Domain.Inventory.Stores;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Commands.RemoveItemUnitFromStore;

public class RemoveItemUnitFromStoreCommandHandler : IRequestHandler<RemoveItemUnitFromStoreCommand, Result<Deleted>>
{
    private readonly ILogger<RemoveItemUnitFromStoreCommandHandler> _logger;
    private readonly IAppDbContext _dbContext;

    public RemoveItemUnitFromStoreCommandHandler(ILogger<RemoveItemUnitFromStoreCommandHandler> logger, IAppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Result<Deleted>> Handle(RemoveItemUnitFromStoreCommand command, CancellationToken ct)
    {
        var itemUnit = await _dbContext.ItemUnits.SingleOrDefaultAsync(iu => iu.ItemId == command.ItemId && iu.UnitId == command.UnitId, ct);
        if (itemUnit is null)
        {
            _logger.LogWarning("ItemUnit not found for ItemId={ItemId} UnitId={UnitId}", command.ItemId, command.UnitId);
            return ItemUnitErrors.ItemUnitNotFound;
        }

        var store = await _dbContext.Stores
            .Include(s => s.StoreItemUnits)
                .ThenInclude(siu => siu.ItemUnit)
            .SingleOrDefaultAsync(s => s.Id == command.StoreId, ct);
        if (store is null)
        {
            _logger.LogWarning("Store not found with id {StoreId}", command.StoreId);
            return StoreErrors.StoreNotFound;
        }

        var removeResult = store.RemoveItemUnit(itemUnit);
        if (removeResult.IsError)
        {
            _logger.LogWarning("Failed to remove item unit from store {StoreId}: {Errors}", command.StoreId, string.Join(',', removeResult.Errors));
            return removeResult.Errors;
        }

        _dbContext.Stores.Update(store);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Removed ItemUnit (ItemId={ItemId},UnitId={UnitId}) from Store {StoreId}", command.ItemId, command.UnitId, command.StoreId);

        return Result.Deleted;
    }
}
