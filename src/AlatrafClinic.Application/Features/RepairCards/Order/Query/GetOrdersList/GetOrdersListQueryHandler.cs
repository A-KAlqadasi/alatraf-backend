using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.RepairCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetOrdersList;

public sealed class GetOrdersListQueryHandler : IRequestHandler<GetOrdersListQuery, Result<List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetOrdersListQueryHandler> _logger;

    public GetOrdersListQueryHandler(IAppDbContext dbContext, ILogger<GetOrdersListQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>>> Handle(GetOrdersListQuery request, CancellationToken ct)
    {
        _logger.LogInformation("Fetching all orders...");

        var orders = await _dbContext.Orders
            .AsNoTracking()
            .Select(o => new AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto
            {
                Id = o.Id,
                RepairCardId = o.RepairCardId,
                SectionId = o.SectionId,
                OrderType = o.OrderType,
                Status = o.Status,
                IsEditable = o.IsEditable
            })
            .ToListAsync(ct);

        if (orders is null || !orders.Any())
        {
            _logger.LogInformation("No orders found.");
            return new List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>();
        }

        _logger.LogInformation("Retrieved {Count} orders.", orders.Count);

        return orders;
    }
}
