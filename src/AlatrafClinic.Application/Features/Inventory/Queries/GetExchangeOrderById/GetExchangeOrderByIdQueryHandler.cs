using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Dtos;
using AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.ExchangeOrders;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Queries.GetExchangeOrderById;

public sealed class GetExchangeOrderByIdQueryHandler : IRequestHandler<GetExchangeOrderByIdQuery, Result<ExchangeOrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetExchangeOrderByIdQueryHandler> _logger;

    public GetExchangeOrderByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetExchangeOrderByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ExchangeOrderDto>> Handle(GetExchangeOrderByIdQuery request, CancellationToken ct)
    {
        _logger.LogInformation("Fetching exchange order by id {ExchangeOrderId}...", request.ExchangeOrderId);

        var exchangeOrder = await _unitOfWork.ExchangeOrders.GetByIdAsync(request.ExchangeOrderId, ct);
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
