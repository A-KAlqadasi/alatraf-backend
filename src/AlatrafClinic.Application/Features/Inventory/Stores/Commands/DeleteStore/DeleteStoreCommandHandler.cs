using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Stores;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Commands.DeleteStore;

public class DeleteStoreCommandHandler : IRequestHandler<DeleteStoreCommand, Result<Deleted>>
{
    private readonly ILogger<DeleteStoreCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteStoreCommandHandler(ILogger<DeleteStoreCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Deleted>> Handle(DeleteStoreCommand command, CancellationToken ct)
    {
        var store = await _unitOfWork.Stores.GetByIdAsync(command.StoreId, ct);
        if (store is null)
        {
            _logger.LogWarning("Store {StoreId} not found", command.StoreId);
            return StoreErrors.StoreNotFound;
        }

        await _unitOfWork.Stores.DeleteAsync(store);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Deleted store {StoreId}", command.StoreId);
        return Result.Deleted;
    }
}
