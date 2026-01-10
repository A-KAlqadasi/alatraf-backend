using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Stores;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Commands.DeleteStore;

public class DeleteStoreCommandHandler : IRequestHandler<DeleteStoreCommand, Result<Deleted>>
{
    private readonly ILogger<DeleteStoreCommandHandler> _logger;
    private readonly IAppDbContext _dbContext;

    public DeleteStoreCommandHandler(ILogger<DeleteStoreCommandHandler> logger, IAppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Result<Deleted>> Handle(DeleteStoreCommand command, CancellationToken ct)
    {
        var store = await _dbContext.Stores.SingleOrDefaultAsync(s => s.Id == command.StoreId, ct);
        if (store is null)
        {
            _logger.LogWarning("Store {StoreId} not found", command.StoreId);
            return StoreErrors.StoreNotFound;
        }
        if (await _dbContext.PurchaseInvoices.AnyAsync(pi => pi.StoreId == store.Id, ct)
            || await _dbContext.ExchangeOrders.AnyAsync(eo => eo.StoreId == store.Id, ct)
            || await _dbContext.SaleItems.AnyAsync(si => _dbContext.StoreItemUnits.Any(siu => siu.Id == si.StoreItemUnitId && siu.StoreId == store.Id), ct))
        {
            _logger.LogWarning("Cannot delete store {StoreId} because it has associated records", command.StoreId);
            return StoreErrors.StoreHasAssociations;
        }

        _dbContext.Stores.Remove(store);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Deleted store {StoreId}", command.StoreId);
        return Result.Deleted;
    }
}
