using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetOrders;

public sealed class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, Result<PaginatedList<OrderDto>>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetOrdersQueryHandler> _logger;

    public GetOrdersQueryHandler(IAppDbContext dbContext, ILogger<GetOrdersQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<OrderDto>>> Handle(GetOrdersQuery request, CancellationToken ct)
    {
        _logger.LogInformation("Building orders query...");

        var query = _dbContext.Orders.AsNoTracking().AsQueryable();

        if (request.SectionId.HasValue)
            query = query.Where(o => o.SectionId == request.SectionId.Value);

        if (request.RepairCardId.HasValue)
            query = query.Where(o => o.RepairCardId == request.RepairCardId.Value);

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (Enum.TryParse<AlatrafClinic.Domain.RepairCards.Enums.OrderStatus>(request.Status, true, out var st))
            {
                query = query.Where(o => o.Status == st);
            }
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var s = request.SearchTerm.Trim();
            if (int.TryParse(s, out var id))
            {
                query = query.Where(o => o.Id == id || (o.RepairCardId.HasValue && o.RepairCardId.Value == id));
            }
        }

        var sortDir = (request.SortDirection ?? "desc").ToLower();
        var sortCol = (request.SortColumn ?? "id").ToLower();

        query = sortCol switch
        {
            "sectionid" => sortDir == "asc" ? query.OrderBy(o => o.SectionId) : query.OrderByDescending(o => o.SectionId),
            "repaircardid" => sortDir == "asc" ? query.OrderBy(o => o.RepairCardId) : query.OrderByDescending(o => o.RepairCardId),
            "status" => sortDir == "asc" ? query.OrderBy(o => o.Status) : query.OrderByDescending(o => o.Status),
            "ordertype" => sortDir == "asc" ? query.OrderBy(o => o.OrderType) : query.OrderByDescending(o => o.OrderType),
            _ => sortDir == "asc" ? query.OrderBy(o => o.Id) : query.OrderByDescending(o => o.Id),
        };

        var totalCount = await query.CountAsync(ct);

        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 200);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                RepairCardId = o.RepairCardId,
                SectionId = o.SectionId,
                OrderType = o.OrderType,
                Status = o.Status,
                IsEditable = o.IsEditable
            })
            .ToListAsync(ct);

        var paginated = new PaginatedList<OrderDto>
        {
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            Items = items
        };

        _logger.LogInformation("Returning {Count} orders (page {Page}/{TotalPages}).", items.Count, page, paginated.TotalPages);

        return paginated;
    }
}
