using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Dtos;
using AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.ExchangeOrders;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Queries.GetExchangeOrderById;

public sealed class GetExchangeOrderByIdQueryHandler : IRequestHandler<GetExchangeOrderByIdQuery, Result<ExchangeOrderDto>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetExchangeOrderByIdQueryHandler> _logger;

    public GetExchangeOrderByIdQueryHandler(IAppDbContext dbContext, ILogger<GetExchangeOrderByIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<ExchangeOrderDto>> Handle(GetExchangeOrderByIdQuery request, CancellationToken ct)
    {
        _logger.LogInformation("Fetching exchange order by id {ExchangeOrderId}...", request.ExchangeOrderId);

        var exchangeOrder = await _dbContext.ExchangeOrders
            .AsNoTracking()
            .Include(e => e.Store)
                .ThenInclude(s => s.StoreItemUnits)
                .ThenInclude(siu => siu.ItemUnit)
                .ThenInclude(iu => iu.Item)
            .Include(e => e.Store)
                .ThenInclude(s => s.StoreItemUnits)
                .ThenInclude(siu => siu.ItemUnit)
                .ThenInclude(iu => iu.Unit)
            .Include(e => e.Items)
                .ThenInclude(i => i.StoreItemUnit)
            .SingleOrDefaultAsync(e => e.Id == request.ExchangeOrderId, ct);
        if (exchangeOrder is null)
        {
            _logger.LogWarning("Exchange order {ExchangeOrderId} not found.", request.ExchangeOrderId);
            return ExchangeOrderErrors.ExchangeOrderRequired;
        }

        var dto = exchangeOrder.ToDto();

        // Enrich dto with store name if needed
        if (exchangeOrder.Store is not null)
        {
            dto.StoreName = exchangeOrder.Store.Name ?? dto.StoreName;
        }

        // If item names are missing (navigation null), try to enrich using loaded store item units if available
        if (dto.Items is not null && exchangeOrder.Store is not null && exchangeOrder.Store.StoreItemUnits is not null)
        {
            var units = exchangeOrder.Store.StoreItemUnits.ToDictionary(s => s.Id, s => s);
            foreach (var it in dto.Items)
            {
                if (units.TryGetValue(it.StoreItemUnitId, out var storeItemUnit))
                {
                    it.ItemName = storeItemUnit.ItemUnit?.Item?.Name ?? it.ItemName;
                    it.UnitName = storeItemUnit.ItemUnit?.Unit?.Name ?? it.UnitName;
                }
            }
        }

        _logger.LogInformation("Returning exchange order {ExchangeOrderId}.", request.ExchangeOrderId);
        return dto;
    }
}
