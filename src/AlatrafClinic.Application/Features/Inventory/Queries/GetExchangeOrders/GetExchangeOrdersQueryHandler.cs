using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Queries.GetExchangeOrders;

public sealed class GetExchangeOrdersQueryHandler : IRequestHandler<GetExchangeOrdersQuery, Result<PaginatedList<ExchangeOrderDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetExchangeOrdersQueryHandler> _logger;

    public GetExchangeOrdersQueryHandler(IUnitOfWork unitOfWork, ILogger<GetExchangeOrdersQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<ExchangeOrderDto>>> Handle(GetExchangeOrdersQuery request, CancellationToken ct)
    {
        _logger.LogInformation("Building exchange orders query...");

        var query = await _unitOfWork.ExchangeOrders.GetExchangeOrdersQueryAsync(ct);

        if (request.OrderId.HasValue)
            query = query.Where(e => e.RelatedOrderId == request.OrderId.Value);

        if (request.SaleId.HasValue)
            query = query.Where(e => e.RelatedSaleId == request.SaleId.Value);

        if (request.StoreId.HasValue)
            query = query.Where(e => e.StoreId == request.StoreId.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var s = request.SearchTerm.Trim();
            if (int.TryParse(s, out var id))
            {
                query = query.Where(e => e.Id == id || e.RelatedOrderId == id || e.RelatedSaleId == id);
            }
            else
            {
                query = query.Where(e => e.Number.Contains(s) || (e.Notes ?? string.Empty).Contains(s) || (e.Store != null && (e.Store.Name ?? string.Empty).Contains(s)));
            }
        }

        var sortDir = (request.SortDirection ?? "desc").ToLower();
        var sortCol = (request.SortColumn ?? "id").ToLower();

        query = sortCol switch
        {
            "number" => sortDir == "asc" ? query.OrderBy(e => e.Number) : query.OrderByDescending(e => e.Number),
            "storeid" => sortDir == "asc" ? query.OrderBy(e => e.StoreId) : query.OrderByDescending(e => e.StoreId),
            "saleid" => sortDir == "asc" ? query.OrderBy(e => e.RelatedSaleId) : query.OrderByDescending(e => e.RelatedSaleId),
            "orderid" => sortDir == "asc" ? query.OrderBy(e => e.RelatedOrderId) : query.OrderByDescending(e => e.RelatedOrderId),
            "isapproved" => sortDir == "asc" ? query.OrderBy(e => e.IsApproved) : query.OrderByDescending(e => e.IsApproved),
            _ => sortDir == "asc" ? query.OrderBy(e => e.Id) : query.OrderByDescending(e => e.Id),
        };

        var totalCount = await query.CountAsync(ct);

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 200);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new ExchangeOrderDto
            {
                Id = e.Id,
                Number = e.Number,
                StoreId = e.StoreId,
                StoreName = e.Store != null ? (e.Store.Name ?? string.Empty) : string.Empty,
                IsApproved = e.IsApproved,
                Notes = e.Notes,
                SaleId = e.RelatedSaleId,
                OrderId = e.RelatedOrderId,
                Items = e.Items.Select(i => new ExchangeOrderItemDto
                {
                    Id = i.Id,
                    ExchangeOrderId = i.ExchangeOrderId,
                    StoreItemUnitId = i.StoreItemUnitId,
                    ItemName = i.StoreItemUnit != null && i.StoreItemUnit.ItemUnit != null && i.StoreItemUnit.ItemUnit.Item != null ? (i.StoreItemUnit.ItemUnit.Item.Name ?? string.Empty) : string.Empty,
                    UnitName = i.StoreItemUnit != null && i.StoreItemUnit.ItemUnit != null && i.StoreItemUnit.ItemUnit.Unit != null ? (i.StoreItemUnit.ItemUnit.Unit.Name ?? string.Empty) : string.Empty,
                    Quantity = i.Quantity
                }).ToList()
            })
            .ToListAsync(ct);

        var paginated = new PaginatedList<ExchangeOrderDto>
        {
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            Items = items
        };

        _logger.LogInformation("Returning {Count} exchange orders (page {Page}/{TotalPages}).", items.Count, page, paginated.TotalPages);

        return paginated;
    }
}
