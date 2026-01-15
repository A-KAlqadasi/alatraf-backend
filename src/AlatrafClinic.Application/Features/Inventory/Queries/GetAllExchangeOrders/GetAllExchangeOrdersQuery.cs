using System;

using AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Queries.GetAllExchangeOrders;

public sealed record GetAllExchangeOrdersQuery(
    int? StoreId,
    int? SaleId,
    int? OrderId,
    string? SearchTerm,
    string? SortColumn,
    string? SortDirection
)
    : IRequest<Result<List<ExchangeOrderDto>>>;
