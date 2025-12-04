using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Dtos;
using AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.ExchangeOrders;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Commands.UpsertExchangeOrderItems;

public sealed class UpsertExchangeOrderItemsCommandHandler : IRequestHandler<UpsertExchangeOrderItemsCommand, Result<ExchangeOrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpsertExchangeOrderItemsCommandHandler> _logger;

    public UpsertExchangeOrderItemsCommandHandler(IUnitOfWork unitOfWork, ILogger<UpsertExchangeOrderItemsCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ExchangeOrderDto>> Handle(UpsertExchangeOrderItemsCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Upserting items for ExchangeOrder {ExchangeOrderId}...", request.ExchangeOrderId);

        var exchangeOrder = await _unitOfWork.ExchangeOrders.GetByIdAsync(request.ExchangeOrderId, ct);
        if (exchangeOrder is null)
        {
            _logger.LogWarning("ExchangeOrder {ExchangeOrderId} not found.", request.ExchangeOrderId);
            return ExchangeOrderErrors.ExchangeOrderRequired;
        }

        var store = await _unitOfWork.Stores.GetByIdAsync(exchangeOrder.StoreId, ct);
        if (store is null)
        {
            _logger.LogWarning("Store {StoreId} not found for ExchangeOrder {ExchangeOrderId}.", exchangeOrder.StoreId, request.ExchangeOrderId);
            return AlatrafClinic.Domain.Inventory.Stores.StoreErrors.StoreNotFound;
        }

        // guard against store item units not loaded
        if (store.StoreItemUnits is null)
        {
            _logger.LogWarning("Store {StoreId} has no StoreItemUnits.", store.Id);
            return AlatrafClinic.Domain.Inventory.Stores.StoreItemUnitErrors.NotFound;
        }

        var unitsDict = store.StoreItemUnits.ToDictionary(s => s.Id, s => s);

        var items = new List<ExchangeOrderItem>();
        foreach (var it in request.Items)
        {
            if (!unitsDict.TryGetValue(it.StoreItemUnitId, out var siu))
            {
                _logger.LogWarning("StoreItemUnit {StoreItemUnitId} not found in store {StoreId}.", it.StoreItemUnitId, store.Id);
                return AlatrafClinic.Domain.Inventory.Stores.StoreItemUnitErrors.NotFound;
            }

            var createResult = ExchangeOrderItem.Create(exchangeOrder.Id, siu.Id, it.Quantity);
            if (createResult.IsError)
            {
                _logger.LogWarning("Invalid ExchangeOrderItem: {Errors}", createResult.Errors);
                return createResult.Errors;
            }
            items.Add(createResult.Value);
        }

        var upsertResult = exchangeOrder.UpsertItems(items);
        if (upsertResult.IsError)
        {
            _logger.LogWarning("ExchangeOrder.UpsertItems returned errors: {Errors}", upsertResult.Errors);
            return upsertResult.Errors;
        }

        await _unitOfWork.ExchangeOrders.UpdateAsync(exchangeOrder, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var dto = exchangeOrder.ToDto();
        dto.StoreName = store.Name;

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

        _logger.LogInformation("Upserted items for ExchangeOrder {ExchangeOrderId} (Count: {Count}).", exchangeOrder.Id, dto.Items?.Count ?? 0);
        return dto;
    }
}
