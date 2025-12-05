using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetOrderItems;

public sealed class GetOrderItemsQueryHandler : IRequestHandler<GetOrderItemsQuery, Result<List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderItemDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetOrderItemsQueryHandler> _logger;

    public GetOrderItemsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetOrderItemsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderItemDto>>> Handle(GetOrderItemsQuery request, CancellationToken ct)
    {
        // Use repository projection to avoid loading the aggregate
        var projected = await _unitOfWork.Orders.GetItemsProjectedAsync(request.OrderId, ct);
        if (projected is null)
        {
            _logger.LogInformation("No items found for Order {OrderId}", request.OrderId);
            return new List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderItemDto>();
        }

        return projected.ToList();
    }
}
