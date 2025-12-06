using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Dtos;
using AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.ExchangeOrders;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Commands.CreateExchangeOrderForSale;

public sealed class CreateExchangeOrderForSaleCommandHandler : IRequestHandler<CreateExchangeOrderForSaleCommand, Result<ExchangeOrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateExchangeOrderForSaleCommandHandler> _logger;

    public CreateExchangeOrderForSaleCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateExchangeOrderForSaleCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ExchangeOrderDto>> Handle(CreateExchangeOrderForSaleCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Creating exchange order for Sale {SaleId}...", request.SaleId);

        // load sale aggregate
        var sale = await _unitOfWork.Sales.GetByIdAsync(request.SaleId, ct);
        if (sale is null)
        {
            _logger.LogWarning("Sale {SaleId} not found.", request.SaleId);
            return AlatrafClinic.Domain.Sales.SaleErrors.SaleNotFound;
        }

        // load store aggregate (with item units for validation and adjustments)
        var store = await _unitOfWork.Stores.GetByIdWithItemUnitsAsync(request.StoreId, ct);
        if (store is null)
        {
            _logger.LogWarning("Store {StoreId} not found.", request.StoreId);
            return AlatrafClinic.Domain.Inventory.Stores.StoreErrors.StoreNotFound;
        }

        if (store.StoreItemUnits is null)
        {
            _logger.LogWarning("Store {StoreId} has no store item units.", store.Id);
            return AlatrafClinic.Domain.Inventory.Stores.StoreItemUnitErrors.NotFound;
        }

        // map incoming items to domain (StoreItemUnit, quantity) tuples
        var saleItems = new List<(AlatrafClinic.Domain.Inventory.Stores.StoreItemUnit StoreItemUnit, decimal Quantity)>();
        foreach (var it in request.Items)
        {
            var siu = store.StoreItemUnits.FirstOrDefault(s => s.Id == it.StoreItemUnitId);
            if (siu is null)
            {
                _logger.LogWarning("StoreItemUnit {StoreItemUnitId} not found in store {StoreId}.", it.StoreItemUnitId, store.Id);
                return AlatrafClinic.Domain.Inventory.Stores.StoreItemUnitErrors.NotFound;
            }
            saleItems.Add((siu, it.Quantity));
        }

        // delegate to domain behavior on Sale aggregate which encapsulates creating, assigning and approving exchange order
        var postResult = sale.Post(request.Number, saleItems, request.Notes);
        if (postResult.IsError)
        {
            _logger.LogWarning("Sale.Post returned errors: {Errors}", postResult.Errors);
            return postResult.Errors;
        }

        var exchangeOrder = sale.ExchangeOrder;
        if (exchangeOrder is null)
        {
            _logger.LogWarning("Sale.Post succeeded but no ExchangeOrder created for Sale {SaleId}.", request.SaleId);
            return ExchangeOrderErrors.ExchangeOrderRequired;
        }

        // persist exchange order and sale (sale status updated inside Post)
        await _unitOfWork.ExchangeOrders.AddAsync(exchangeOrder, ct);

        // decrease store quantities through Store aggregate methods
        foreach (var line in exchangeOrder.Items)
        {
            var storeItem = store.StoreItemUnits.FirstOrDefault(s => s.Id == line.StoreItemUnitId);
            if (storeItem is null)
            {
                _logger.LogWarning("StoreItemUnit {StoreItemUnitId} not found in Store {StoreId} during sale post.", line.StoreItemUnitId, store.Id);
                return AlatrafClinic.Domain.Inventory.Stores.StoreItemUnitErrors.NotFound;
            }
            var dec = store.AdjustItemUnit(storeItem.ItemUnit, -line.Quantity);
            if (dec.IsError)
            {
                _logger.LogWarning("Failed to decrease store item unit {StoreItemUnitId}: {Errors}", storeItem.Id, dec.Errors);
                return dec.Errors;
            }
        }
        await _unitOfWork.Stores.UpdateAsync(store, ct);
        await _unitOfWork.Sales.UpdateAsync(sale, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var dto = exchangeOrder.ToDto();
        dto.StoreName = store.Name;

        // ensure item names/units set using store data
        if (dto.Items is not null && store.StoreItemUnits is not null)
        {
            var units = store.StoreItemUnits.ToDictionary(s => s.Id, s => s);
            foreach (var it in dto.Items)
            {
                if (units.TryGetValue(it.StoreItemUnitId, out var storeItemUnit))
                {
                    it.ItemName = storeItemUnit.ItemUnit?.Item?.Name ?? it.ItemName;
                    it.UnitName = storeItemUnit.ItemUnit?.Unit?.Name ?? it.UnitName;
                }
            }
        }

        _logger.LogInformation("Exchange order {ExchangeOrderId} created and assigned to sale {SaleId}.", exchangeOrder.Id, request.SaleId);
        return dto;
    }
}
