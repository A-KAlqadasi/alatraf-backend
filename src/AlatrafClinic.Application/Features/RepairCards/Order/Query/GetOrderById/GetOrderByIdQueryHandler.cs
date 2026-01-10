using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Application.Features.RepairCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards.Orders;

using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderDto>>
{
    private readonly ILogger<GetOrderByIdQueryHandler> _logger;
    private readonly IAppDbContext _dbContext;

    public GetOrderByIdQueryHandler(ILogger<GetOrderByIdQueryHandler> logger, IAppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery query, CancellationToken ct)
    {
        var order = await _dbContext.Orders.AsNoTracking().SingleOrDefaultAsync(o => o.Id == query.OrderId, ct);
        if (order is null)
        {
            _logger.LogWarning("Order with Id {OrderId} not found.", query.OrderId);
            return OrderErrors.OrderNotFound;
        }

        return order.ToDto();
    }
}
