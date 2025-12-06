using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Stores;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Commands.UpdateStore;

public class UpdateStoreCommandHandler : IRequestHandler<UpdateStoreCommand, Result<Updated>>
{
    private readonly ILogger<UpdateStoreCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStoreCommandHandler(ILogger<UpdateStoreCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Updated>> Handle(UpdateStoreCommand command, CancellationToken ct)
    {
        var store = await _unitOfWork.Stores.GetByIdAsync(command.StoreId, ct);
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

        await _unitOfWork.Stores.UpdateAsync(store, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Updated store {StoreId}", command.StoreId);
        return Result.Updated;
    }
}
