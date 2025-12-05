using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Application.Features.RepairCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards.Orders;

using MediatR;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderDto>>
{
    private readonly ILogger<GetOrderByIdQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetOrderByIdQueryHandler(ILogger<GetOrderByIdQueryHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery query, CancellationToken ct)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(query.OrderId, ct);
        if (order is null)
        {
            _logger.LogWarning("Order with Id {OrderId} not found.", query.OrderId);
            return OrderErrors.OrderNotFound;
        }

        return order.ToDto();
    }
}
