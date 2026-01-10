using System;

using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

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
    ) : IRequest<Result<PaginatedList<ExchangeOrderDto>>>;