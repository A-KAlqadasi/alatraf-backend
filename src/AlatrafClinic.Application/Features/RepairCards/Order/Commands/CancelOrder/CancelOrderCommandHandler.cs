using Microsoft.Extensions.Logging;

using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using AlatrafClinic.Domain.Orders;

using Microsoft.EntityFrameworkCore;


namespace AlatrafClinic.Application.Features.RepairCards.Commands.CancelOrder;

public sealed class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result<Updated>>
{
    private readonly ILogger<CancelOrderCommandHandler> _logger;
    private readonly IAppDbContext _dbContext;

    public CancelOrderCommandHandler(ILogger<CancelOrderCommandHandler> logger, IAppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Result<Updated>> Handle(CancelOrderCommand command, CancellationToken ct)
    {
        var order = await _dbContext.Orders.SingleOrDefaultAsync(o => o.Id == command.OrderId, ct);

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

        _dbContext.Orders.Update(order);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Order {OrderId} cancelled successfully.", command.OrderId);
        return Result.Updated;
    }
}
