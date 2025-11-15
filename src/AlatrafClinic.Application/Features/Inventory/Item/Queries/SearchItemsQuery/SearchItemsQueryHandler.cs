using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Items.Queries.SearchItemsQuery;

public sealed class SearchItemsQueryHandler : IRequestHandler<SearchItemsQuery, Result<PagedList<ItemDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SearchItemsQueryHandler> _logger;

    public SearchItemsQueryHandler(IUnitOfWork unitOfWork, ILogger<SearchItemsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
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

        // ✅ تنفيذ الاستعلام عبر الـ Repository
        var result = await _unitOfWork.Items.SearchAsync(spec, cancellationToken);

        if (!result.Items.Any())
        {
            _logger.LogWarning("No items found matching the search criteria.");
            return new PagedList<ItemDto>(new List<ItemDto>(), 0, request.Page, request.PageSize);
        }

        // ✅ تحويل النتائج إلى DTOs
        var itemDtos = result.Items.Select(i => i.ToDto()).ToList();
        var pagedList = new PagedList<ItemDto>(itemDtos, result.TotalCount, request.Page, request.PageSize);

        _logger.LogInformation("SearchItemsQuery returned {Count} items.", pagedList.Items.Count);

        return pagedList;
    }
}
