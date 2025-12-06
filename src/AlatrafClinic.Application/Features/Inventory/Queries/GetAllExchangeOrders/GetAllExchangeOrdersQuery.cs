using System;

using AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Inventory.Queries.GetAllExchangeOrders;

public sealed record GetAllExchangeOrdersQuery(
    int? StoreId,
    int? SaleId,
    int? OrderId,
    string? SearchTerm,
    string? SortColumn,
    string? SortDirection
)
    : AlatrafClinic.Application.Common.Interfaces.ICachedQuery<Result<List<ExchangeOrderDto>>>
{
    public string CacheKey => "exchange_orders_all";
    public string[] Tags => new[] { "exchange_order" };
    public TimeSpan Expiration => TimeSpan.FromMinutes(2);
}
