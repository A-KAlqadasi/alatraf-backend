using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetOrderItems;

public sealed class GetOrderItemsQueryHandler : IRequestHandler<GetOrderItemsQuery, Result<List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderItemDto>>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetOrderItemsQueryHandler> _logger;

    public GetOrderItemsQueryHandler(IAppDbContext dbContext, ILogger<GetOrderItemsQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderItemDto>>> Handle(GetOrderItemsQuery request, CancellationToken ct)
    {
        // Project directly from DbContext to avoid loading aggregate
        var projected = await _dbContext.OrderItems
            .AsNoTracking()
            .Where(oi => oi.OrderId == request.OrderId)
            .Select(oi => new AlatrafClinic.Application.Features.RepairCards.Dtos.OrderItemDto
            {
                Id = oi.Id,
                ItemId = oi.ItemUnit.ItemId,
                ItemName = oi.ItemUnit.Item.Name,
                UnitId = oi.ItemUnit.UnitId,
                UnitName = oi.ItemUnit.Unit.Name,
                Quantity = oi.Quantity,
                UnitPrice = oi.Price,
                Total = oi.Quantity * oi.Price
            })
            .ToListAsync(ct);
        if (projected is null)
        {
            _logger.LogInformation("No items found for Order {OrderId}", request.OrderId);
            return new List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderItemDto>();
        }

        return projected.ToList();
    }
}
