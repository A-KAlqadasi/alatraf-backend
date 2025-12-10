using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.ExchangeOrders;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Commands.ApproveExchangeOrder;

public sealed class ApproveExchangeOrderCommandHandler : IRequestHandler<ApproveExchangeOrderCommand, Result<Updated>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ApproveExchangeOrderCommandHandler> _logger;

    public ApproveExchangeOrderCommandHandler(IUnitOfWork unitOfWork, ILogger<ApproveExchangeOrderCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Updated>> Handle(ApproveExchangeOrderCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Approving exchange order {ExchangeOrderId}...", request.ExchangeOrderId);

        var exchangeOrder = await _unitOfWork.ExchangeOrders.GetByIdAsync(request.ExchangeOrderId, ct);
        if (exchangeOrder is null)
        {
            _logger.LogWarning("Exchange order {ExchangeOrderId} not found.", request.ExchangeOrderId);
            return ExchangeOrderErrors.ExchangeOrderRequired;
        }

        var approval = exchangeOrder.Approve();
        if (approval.IsError)
        {
            _logger.LogWarning("ExchangeOrder.Approve returned errors: {Errors}", approval.Errors);
            return approval.Errors;
        }

        // load store aggregate with item units so we can apply stock adjustments
        var store = await _unitOfWork.Stores.GetByIdWithItemUnitsAsync(exchangeOrder.StoreId, ct);
        if (store is null)
        {
            _logger.LogWarning("Store {StoreId} not found when approving exchange order {ExchangeOrderId}.", exchangeOrder.StoreId, exchangeOrder.Id);
            return AlatrafClinic.Domain.Inventory.Stores.StoreErrors.StoreNotFound;
        }

        foreach (var line in exchangeOrder.Items)
        {
            var storeItem = store.StoreItemUnits.FirstOrDefault(s => s.Id == line.StoreItemUnitId);
            if (storeItem is null)
            {
                _logger.LogWarning("StoreItemUnit {StoreItemUnitId} not found in Store {StoreId} during approve.", line.StoreItemUnitId, store.Id);
                return AlatrafClinic.Domain.Inventory.Stores.StoreItemUnitErrors.NotFound;
            }

            var dec = store.AdjustItemUnit(storeItem.ItemUnit, -line.Quantity);
            if (dec.IsError)
            {
                _logger.LogWarning("Failed to decrease store item unit {StoreItemUnitId}: {Errors}", storeItem.Id, dec.Errors);
                return dec.Errors;
            }
        }

        await _unitOfWork.Stores.UpdateAsync(store, ct);
        await _unitOfWork.ExchangeOrders.UpdateAsync(exchangeOrder, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Exchange order {ExchangeOrderId} approved.", request.ExchangeOrderId);
        return Result.Updated;
    }
}
