using System.Linq;
using AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Dtos;
using AlatrafClinic.Domain.Inventory.ExchangeOrders;

namespace AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Mappers;

public static class ExchangeOrderMapper
{
    public static ExchangeOrderDto ToDto(this ExchangeOrder exchangeOrder)
    {
        if (exchangeOrder == null) return null!;

        var dto = new ExchangeOrderDto
        {
            Id = exchangeOrder.Id,
            Number = exchangeOrder.Number,
            StoreId = exchangeOrder.StoreId,
            StoreName = exchangeOrder.Store?.Name ?? string.Empty,
            IsApproved = exchangeOrder.IsApproved,
            Notes = exchangeOrder.Notes,
            SaleId = exchangeOrder.RelatedSaleId,
            OrderId = exchangeOrder.RelatedOrderId,
            Items = exchangeOrder.Items.Select(i => new ExchangeOrderItemDto
            {
                Id = i.Id,
                ExchangeOrderId = i.ExchangeOrderId,
                StoreItemUnitId = i.StoreItemUnitId,
                ItemName = i.StoreItemUnit?.ItemUnit?.Item?.Name ?? string.Empty,
                UnitName = i.StoreItemUnit?.ItemUnit?.Unit?.Name ?? string.Empty,
                Quantity = i.Quantity
            }).ToList()
        };

        return dto;
    }
}
