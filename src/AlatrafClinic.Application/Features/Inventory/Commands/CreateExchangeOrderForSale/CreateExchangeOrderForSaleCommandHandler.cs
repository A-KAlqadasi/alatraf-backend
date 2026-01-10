using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Dtos;
using AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.ExchangeOrders;
using AlatrafClinic.Domain.Inventory.Stores;
using AlatrafClinic.Domain.Sales;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Commands.CreateExchangeOrderForSale;

public sealed class CreateExchangeOrderForSaleCommandHandler
    : IRequestHandler<CreateExchangeOrderForSaleCommand, Result<ExchangeOrderDto>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<CreateExchangeOrderForSaleCommandHandler> _logger;

    public CreateExchangeOrderForSaleCommandHandler(
        IAppDbContext dbContext,
        ILogger<CreateExchangeOrderForSaleCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<Result<ExchangeOrderDto>> Handle(CreateExchangeOrderForSaleCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Creating exchange order for sale…");

        // 1) Load sale
        var sale = await _dbContext.Sales
            .Include(s => s.Payment)
            .Include(s => s.SaleItems)
            .SingleOrDefaultAsync(s => s.Id == request.SaleId, ct);
        if (sale is null)
            return SaleErrors.SaleNotFound;

        // 2) Validate payment
        if (sale.Payment is null)
            return SaleErrors.PaymentNotFound;
        if (!sale.Payment.IsCompleted)
            return SaleErrors.PaymentNotCompleted;

        // 3) Load store with units
        var store = await _dbContext.Stores
            .Include(s => s.StoreItemUnits)
            .ThenInclude(siu => siu.ItemUnit)
            .ThenInclude(iu => iu.Item)
            .Include(s => s.StoreItemUnits)
            .ThenInclude(siu => siu.ItemUnit)
            .ThenInclude(iu => iu.Unit)
            .SingleOrDefaultAsync(s => s.Id == request.StoreId, ct);
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
        await _dbContext.ExchangeOrders.AddAsync(exchangeOrder, ct);

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

        // store tracked in context

        // 8) Save changes
        await _dbContext.SaveChangesAsync(ct);

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