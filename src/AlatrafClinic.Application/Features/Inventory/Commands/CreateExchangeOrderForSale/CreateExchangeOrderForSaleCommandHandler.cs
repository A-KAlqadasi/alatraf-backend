using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Dtos;
using AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.ExchangeOrders;
using AlatrafClinic.Domain.Inventory.Stores;
using AlatrafClinic.Domain.Sales;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Commands.CreateExchangeOrderForSale;

public sealed class CreateExchangeOrderForSaleCommandHandler
    : IRequestHandler<CreateExchangeOrderForSaleCommand, Result<ExchangeOrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateExchangeOrderForSaleCommandHandler> _logger;

    public CreateExchangeOrderForSaleCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CreateExchangeOrderForSaleCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task<Result<ExchangeOrderDto>> Handle(CreateExchangeOrderForSaleCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Creating exchange order for sale…");

        // 1) Load sale
        var sale = await _unitOfWork.Sales.GetByIdAsync(request.SaleId, ct);
        if (sale is null)
            return SaleErrors.SaleNotFound;

        // 2) Validate payment
        if (sale.Payment is null)
            return SaleErrors.PaymentNotFound;
        if (!sale.Payment.IsCompleted)
            return SaleErrors.PaymentNotCompleted;

        // 3) Load store with units
        var store = await _unitOfWork.Stores.GetByIdWithItemUnitsAsync(request.StoreId, ct);
        if (store is null)
            return StoreErrors.StoreNotFound;
        if (store.StoreItemUnits == null)
            return StoreItemUnitErrors.NotFound;

        // 4) Build ExchangeOrderItems from sale items
        var exchangeOrderItems = new List<ExchangeOrderItem>();
        foreach (var saleItem in sale.SaleItems)
        {
            var storeUnit = store.StoreItemUnits.FirstOrDefault(u => u.ItemUnitId == saleItem.ItemUnitId);
            if (storeUnit is null)
                return StoreItemUnitErrors.NotFound;

            var itemResult = ExchangeOrderItem.Create(storeUnit.Id, saleItem.Quantity);
            if (itemResult.IsError)
                return itemResult.Errors;

            exchangeOrderItems.Add(itemResult.Value);
        }

        // 5) Create ExchangeOrder as AGGREGATE ROOT
        var createResult = ExchangeOrder.CreateForSale(
            saleId: sale.Id,
            storeId: store.Id,
            number: request.Number,
            items: exchangeOrderItems,
            notes: request.Notes
        );

        if (createResult.IsError)
            return createResult.Errors;

        var exchangeOrder = createResult.Value;

        // 6) Persist Exchange Order
        await _unitOfWork.ExchangeOrders.AddAsync(exchangeOrder, ct);

        // 7) Apply store quantity deduction
        foreach (var line in exchangeOrder.Items)
        {
            var storeUnit = store.StoreItemUnits.FirstOrDefault(s => s.Id == line.StoreItemUnitId);
            if (storeUnit == null)
                return StoreItemUnitErrors.NotFound;

            var dec = store.AdjustItemUnit(storeUnit.ItemUnit, -line.Quantity);
            if (dec.IsError)
                return dec.Errors;
        }

        await _unitOfWork.Stores.UpdateAsync(store, ct);

        // 8) Save changes
        await _unitOfWork.SaveChangesAsync(ct);

        // 9) Map to DTO
        var dto = exchangeOrder.ToDto();
        dto.StoreName = store.Name;

        if (dto.Items is null)
            return StoreItemUnitErrors.NotFound;

        foreach (var item in dto.Items)
        {
            var storeUnit = store.StoreItemUnits.FirstOrDefault(x => x.Id == item.StoreItemUnitId);
            if (storeUnit != null)
            {
                item.ItemName = storeUnit.ItemUnit?.Item?.Name;
                item.UnitName = storeUnit.ItemUnit?.Unit?.Name;
            }
        }

        _logger.LogInformation(
            "Exchange order {ExchangeOrderId} created for sale {SaleId}",
            exchangeOrder.Id,
            sale.Id);

        return dto;
    }
}