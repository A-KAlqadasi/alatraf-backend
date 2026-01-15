using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Stores;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Commands.UpdateStore;

public class UpdateStoreCommandHandler : IRequestHandler<UpdateStoreCommand, Result<Updated>>
{
    private readonly ILogger<UpdateStoreCommandHandler> _logger;
    private readonly IAppDbContext _dbContext;

    public UpdateStoreCommandHandler(ILogger<UpdateStoreCommandHandler> logger, IAppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Result<Updated>> Handle(UpdateStoreCommand command, CancellationToken ct)
    {
        var store = await _dbContext.Stores.SingleOrDefaultAsync(s => s.Id == command.StoreId, ct);
        if (store is null)
        {
            _logger.LogWarning("Store {StoreId} not found", command.StoreId);
            return StoreErrors.StoreNotFound;
        }

        var updateResult = store.Update(command.Name);
        if (updateResult.IsError)
        {
            _logger.LogWarning("Failed to update store {StoreId}: {Errors}", command.StoreId, string.Join(',', updateResult.Errors));
            return updateResult.Errors;
        }

        _dbContext.Stores.Update(store);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Updated store {StoreId}", command.StoreId);
        return Result.Updated;
    }
}
