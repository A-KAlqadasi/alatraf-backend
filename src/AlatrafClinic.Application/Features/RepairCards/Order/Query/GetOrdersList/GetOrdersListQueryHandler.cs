using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.RepairCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetOrdersList;

public sealed class GetOrdersListQueryHandler : IRequestHandler<GetOrdersListQuery, Result<List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetOrdersListQueryHandler> _logger;

    public GetOrdersListQueryHandler(IUnitOfWork unitOfWork, ILogger<GetOrdersListQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>>> Handle(GetOrdersListQuery request, CancellationToken ct)
    {
        _logger.LogInformation("Fetching all orders...");

        var orders = await _unitOfWork.Orders.GetAllAsync(ct);

        if (orders is null || !orders.Any())
        {
            _logger.LogInformation("No orders found.");
            return new List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>();
        }

        var dtos = orders.Select(o => o.ToDto()).ToList();

        _logger.LogInformation("Retrieved {Count} orders.", dtos.Count);

        return dtos;
    }
}
