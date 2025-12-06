using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Dtos;
using AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.ExchangeOrders;
using AlatrafClinic.Domain.RepairCards.Orders;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Commands.CreateExchangeOrderForOrder;

public sealed class CreateExchangeOrderForOrderCommandHandler : IRequestHandler<CreateExchangeOrderForOrderCommand, Result<ExchangeOrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateExchangeOrderForOrderCommandHandler> _logger;

    public CreateExchangeOrderForOrderCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateExchangeOrderForOrderCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ExchangeOrderDto>> Handle(CreateExchangeOrderForOrderCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Creating exchange order for Order {OrderId}...", request.OrderId);

        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, ct);
        if (order is null)
        {
            _logger.LogWarning("Order {OrderId} not found.", request.OrderId);
            return AlatrafClinic.Domain.RepairCards.Orders.OrderErrors.OrderNotFound;
        }

        var store = await _unitOfWork.Stores.GetByIdWithItemUnitsAsync(request.StoreId, ct);
        if (store is null)
        {
            _logger.LogWarning("Store {StoreId} not found.", request.StoreId);
            return AlatrafClinic.Domain.Inventory.Stores.StoreErrors.StoreNotFound;
        }

        if (store.StoreItemUnits is null)
        {
            _logger.LogWarning("Store {StoreId} has no StoreItemUnits.", store.Id);
            return AlatrafClinic.Domain.Inventory.Stores.StoreItemUnitErrors.NotFound;
        }

        var createResult = ExchangeOrder.Create(request.StoreId, request.Notes);
        if (createResult.IsError)
        {
            _logger.LogWarning("Failed creating exchange order: {Errors}", createResult.Errors);
            return createResult.Errors;
        }
        var exchangeOrder = createResult.Value;

        var unitsDict = store.StoreItemUnits.ToDictionary(s => s.Id, s => s);
        var items = new List<ExchangeOrderItem>();
        foreach (var it in request.Items)
        {
            if (!unitsDict.TryGetValue(it.StoreItemUnitId, out var siu))
            {
                _logger.LogWarning("StoreItemUnit {StoreItemUnitId} not found in Store {StoreId}.", it.StoreItemUnitId, store.Id);
                return AlatrafClinic.Domain.Inventory.Stores.StoreItemUnitErrors.NotFound;
            }

            var createItem = ExchangeOrderItem.Create(exchangeOrder.Id, siu.Id, it.Quantity);
            if (createItem.IsError)
            {
                _logger.LogWarning("Invalid exchange order item: {Errors}", createItem.Errors);
                return createItem.Errors;
            }
            // Do not mutate store navs here; store adjustments will be done by Store aggregate methods below.
            items.Add(createItem.Value);
        }

        var upsertResult = exchangeOrder.UpsertItems(items);
        if (upsertResult.IsError)
        {
            _logger.LogWarning("Failed upserting items to exchange order: {Errors}", upsertResult.Errors);
            return upsertResult.Errors;
        }

        var assignResult = exchangeOrder.AssignOrder(order.Id, request.Number);
        if (assignResult.IsError)
        {
            _logger.LogWarning("Failed assigning order to exchange order: {Errors}", assignResult.Errors);
            return assignResult.Errors;
        }

        await _unitOfWork.ExchangeOrders.AddAsync(exchangeOrder, ct);

        // approve exchange order (decrease stock) via domain method
        var exchangeApproveResult = exchangeOrder.Approve();
        if (exchangeApproveResult.IsError)
        {
            _logger.LogWarning("Failed approving exchange order {ExchangeOrderId}: {Errors}", exchangeOrder.Id, exchangeApproveResult.Errors);
            return exchangeApproveResult.Errors;
        }

        // Now decrease store quantities through Store aggregate methods (so store owns the mutation)
        foreach (var line in exchangeOrder.Items)
        {
            var storeItem = store.StoreItemUnits.FirstOrDefault(s => s.Id == line.StoreItemUnitId);
            if (storeItem is null)
            {
                _logger.LogWarning("StoreItemUnit {StoreItemUnitId} not found in Store {StoreId} during approve.", line.StoreItemUnitId, store.Id);
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

        // approve order and persist the change via repository
        var approveOrderResult = order.Approve();
        if (approveOrderResult.IsError)
        {
            _logger.LogWarning("Failed approving order {OrderId}: {Errors}", order.Id, approveOrderResult.Errors);
            return approveOrderResult.Errors;
        }
        await _unitOfWork.Orders.UpdateAsync(order, ct);
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

        _logger.LogInformation("Exchange order {ExchangeOrderId} created and assigned to order {OrderId}.", exchangeOrder.Id, request.OrderId);
        return dto;
    }
}
