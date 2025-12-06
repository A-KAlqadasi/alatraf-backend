using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.ExchangeOrders;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Commands.CancelExchangeOrder;

public sealed class CancelExchangeOrderCommandHandler : IRequestHandler<CancelExchangeOrderCommand, Result<Deleted>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelExchangeOrderCommandHandler> _logger;

    public CancelExchangeOrderCommandHandler(IUnitOfWork unitOfWork, ILogger<CancelExchangeOrderCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Deleted>> Handle(CancelExchangeOrderCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Cancelling exchange order {ExchangeOrderId}...", request.ExchangeOrderId);

        var exchangeOrder = await _unitOfWork.ExchangeOrders.GetByIdAsync(request.ExchangeOrderId, ct);
        if (exchangeOrder is null)
        {
            _logger.LogWarning("Exchange order {ExchangeOrderId} not found.", request.ExchangeOrderId);
            return ExchangeOrderErrors.ExchangeOrderRequired;
        }

        // cannot cancel approved exchange orders
        if (exchangeOrder.IsApproved)
        {
            _logger.LogWarning("Exchange order {ExchangeOrderId} is already approved and cannot be cancelled.", request.ExchangeOrderId);
            return ExchangeOrderErrors.AlreadyApproved;
        }

        // if assigned to sale or order, prevent cancellation (use domain errors)
        if (exchangeOrder.RelatedSaleId is not null)
        {
            _logger.LogWarning("Exchange order {ExchangeOrderId} is assigned to sale {SaleId} and cannot be cancelled.", request.ExchangeOrderId, exchangeOrder.RelatedSaleId);
            return ExchangeOrderErrors.ExchangeOrderAlreadyAssignedToSales;
        }
        if (exchangeOrder.RelatedOrderId is not null)
        {
            _logger.LogWarning("Exchange order {ExchangeOrderId} is assigned to order {OrderId} and cannot be cancelled.", request.ExchangeOrderId, exchangeOrder.RelatedOrderId);
            return ExchangeOrderErrors.ExchangeOrderAlreadyAssignedToOrder;
        }

        await _unitOfWork.ExchangeOrders.DeleteAsync(exchangeOrder, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Exchange order {ExchangeOrderId} cancelled.", request.ExchangeOrderId);
        return Result.Deleted;
    }
}
