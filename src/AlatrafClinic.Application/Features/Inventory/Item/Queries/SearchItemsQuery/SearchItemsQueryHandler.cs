using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Items.Queries.SearchItemsQuery;

public sealed class SearchItemsQueryHandler : IRequestHandler<SearchItemsQuery, Result<PagedList<ItemDto>>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<SearchItemsQueryHandler> _logger;

    public SearchItemsQueryHandler(IAppDbContext dbContext, ILogger<SearchItemsQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<PagedList<ItemDto>>> Handle(SearchItemsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Executing item search with filters: {@Filters}", request);

        // ✅ بناء مواصفات البحث (Specification)
        var spec = new ItemSearchSpec(
            request.Keyword,
            request.BaseUnitId,
            request.UnitId,
            request.MinPrice,
            request.MaxPrice,
            request.IsActive,
            request.SortBy,
            request.SortDir,
            request.Page,
            request.PageSize
        );

        var query = _dbContext.Items.AsQueryable();

        if (!string.IsNullOrWhiteSpace(spec.Keyword))
        {
            var kw = spec.Keyword.Trim();
            query = query.Where(i => i.Name.Contains(kw));
        }

        if (spec.BaseUnitId.HasValue)
            query = query.Where(i => i.BaseUnitId == spec.BaseUnitId.Value);

        if (spec.IsActive.HasValue)
            query = query.Where(i => i.IsActive == spec.IsActive.Value);

        if (spec.UnitId.HasValue || spec.MinPrice.HasValue || spec.MaxPrice.HasValue)
        {
            if (spec.UnitId.HasValue)
            {
                var uid = spec.UnitId.Value;
                query = query.Where(i => i.ItemUnits.Any(iu => iu.UnitId == uid));
            }

            if (spec.MinPrice.HasValue)
            {
                var min = spec.MinPrice.Value;
                query = query.Where(i => i.ItemUnits.Any(iu => iu.Price >= min));
            }

            if (spec.MaxPrice.HasValue)
            {
                var max = spec.MaxPrice.Value;
                query = query.Where(i => i.ItemUnits.Any(iu => iu.Price <= max));
            }
        }

        var total = await query.CountAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(spec.SortBy))
        {
            var dir = spec.SortDir?.ToLowerInvariant() == "desc";
            if (spec.SortBy.Equals("name", StringComparison.OrdinalIgnoreCase))
                query = dir ? query.OrderByDescending(i => i.Name) : query.OrderBy(i => i.Name);
            else
                query = query.OrderBy(i => i.Id);
        }
        else
        {
            query = query.OrderBy(i => i.Name);
        }

        var skip = (spec.Page - 1) * spec.PageSize;
        var items = await query.Skip(skip).Take(spec.PageSize).ToListAsync(cancellationToken);

        if (!items.Any())
        {
            _logger.LogWarning("No items found matching the search criteria.");
            return new PagedList<ItemDto>(new List<ItemDto>(), 0, request.Page, request.PageSize);
        }

        // ✅ تحويل النتائج إلى DTOs
        var itemDtos = items.Select(i => i.ToDto()).ToList();
        var pagedList = new PagedList<ItemDto>(itemDtos, total, request.Page, request.PageSize);

        _logger.LogInformation("SearchItemsQuery returned {Count} items.", pagedList.Items.Count);

        return pagedList;
    }
}
