using System;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Inventory.Queries.GetExchangeOrders;

public sealed record GetExchangeOrdersQuery(
    int? OrderId,
    int? SaleId,
    int? StoreId,
    string? SearchTerm,
    string? SortColumn,
    string? SortDirection,
    int Page = 1,
    int PageSize = 10
    ) : AlatrafClinic.Application.Common.Interfaces.ICachedQuery<Result<PaginatedList<ExchangeOrderDto>>>
{
    public string CacheKey => "exchange_orders_list";
    public string[] Tags => new[] { "exchange_order" };
    public TimeSpan Expiration => TimeSpan.FromMinutes(2);
}
