using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Dtos;
using AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.ExchangeOrders;
using AlatrafClinic.Domain.Inventory.Stores;
using AlatrafClinic.Domain.Orders;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Commands.CreateExchangeOrderForOrder;

public sealed class CreateExchangeOrderForOrderCommandHandler
    : IRequestHandler<CreateExchangeOrderForOrderCommand, Result<ExchangeOrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateExchangeOrderForOrderCommandHandler> _logger;

    public CreateExchangeOrderForOrderCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CreateExchangeOrderForOrderCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ExchangeOrderDto>> Handle(
        CreateExchangeOrderForOrderCommand request,
        CancellationToken ct)
    {
        _logger.LogInformation("Creating exchange order for Order {OrderId}...", request.OrderId);

        // 1) Load order
        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, ct);
        if (order is null)
            return OrderErrors.OrderNotFound;

        // 2) Load store with item units
        var store = await _unitOfWork.Stores.GetByIdWithItemUnitsAsync(request.StoreId, ct);
        if (store is null) return StoreErrors.StoreNotFound;
        if (store.StoreItemUnits == null || !store.StoreItemUnits.Any())
            return StoreItemUnitErrors.NotFound;

        // 3) Build ExchangeOrderItems
        var exchangeOrderItems = new List<ExchangeOrderItem>();
        var unitsDict = store.StoreItemUnits.ToDictionary(u => u.Id);
        foreach (var item in request.Items)
        {
            if (!unitsDict.TryGetValue(item.StoreItemUnitId, out var storeUnit))
                return StoreItemUnitErrors.NotFound;

            if (storeUnit.ItemUnit is null)
                return StoreItemUnitErrors.ItemUnitNotFound;

            var itemResult = ExchangeOrderItem.Create(storeUnit.Id, item.Quantity); // aggregate sets ExchangeOrderId later
            if (itemResult.IsError) return itemResult.Errors;

            exchangeOrderItems.Add(itemResult.Value);
        }

        // 4) Create ExchangeOrder aggregate
        var createResult = ExchangeOrder.CreateForOrder(
            orderId: order.Id,
            storeId: store.Id,
            number: request.Number,
            items: exchangeOrderItems,
            notes: request.Notes
        );
        if (createResult.IsError) return createResult.Errors;

        var exchangeOrder = createResult.Value;

        // 5) Persist ExchangeOrder
        await _unitOfWork.ExchangeOrders.AddAsync(exchangeOrder, ct);

        // 6) Approve ExchangeOrder
        var approveResult = exchangeOrder.Approve();
        if (approveResult.IsError) return approveResult.Errors;

        // 7) Adjust store stock
        foreach (var line in exchangeOrder.Items)
        {
            var storeUnit = store.StoreItemUnits.FirstOrDefault(s => s.Id == line.StoreItemUnitId);
            if (storeUnit is null) return StoreItemUnitErrors.NotFound;

            if (storeUnit.ItemUnit is null)
                return StoreItemUnitErrors.ItemUnitNotFound;

            var adjustResult = store.AdjustItemUnit(storeUnit.ItemUnit, -line.Quantity);
            if (adjustResult.IsError) return adjustResult.Errors;
        }

        await _unitOfWork.Stores.UpdateAsync(store, ct);

        // 8) Approve order
        var orderApprove = order.Approve();
        if (orderApprove.IsError) return orderApprove.Errors;
        await _unitOfWork.Orders.UpdateAsync(order, ct);

        // 9) Save changes
        await _unitOfWork.SaveChangesAsync(ct);

        // 10) Map to DTO
        var dto = exchangeOrder.ToDto();
        dto.StoreName = store.Name;
        if(dto.Items is null)
            return StoreItemUnitErrors.NotFound;

        foreach (var item in dto.Items)
        {
            if (unitsDict.TryGetValue(item.StoreItemUnitId, out var storeUnit))
            {
                item.ItemName = storeUnit.ItemUnit?.Item?.Name;
                item.UnitName = storeUnit.ItemUnit?.Unit?.Name;
            }
        }

        _logger.LogInformation("Exchange order {ExchangeOrderId} created for order {OrderId}", exchangeOrder.Id, order.Id);
        return dto;
    }
}
