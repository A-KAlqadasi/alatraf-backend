using Microsoft.Extensions.Logging;

using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;

using MediatR;
using AlatrafClinic.Domain.Orders;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.CancelOrder;

public sealed class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result<Updated>>
{
    private readonly ILogger<CancelOrderCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CancelOrderCommandHandler(ILogger<CancelOrderCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Updated>> Handle(CancelOrderCommand command, CancellationToken ct)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(command.OrderId, ct);

        if (order is null)
        {
            _logger.LogWarning("Order with Id {OrderId} not found.", command.OrderId);
            return OrderErrors.OrderNotFound;
        }

        var result = order.Cancel();
        if (result.IsError)
        {
            _logger.LogWarning("Failed to cancel Order {OrderId}: {Errors}", command.OrderId, result.Errors);
            return result.Errors;
        }

        await _unitOfWork.Orders.UpdateAsync(order, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Order {OrderId} cancelled successfully.", command.OrderId);
        return Result.Updated;
    }
}
