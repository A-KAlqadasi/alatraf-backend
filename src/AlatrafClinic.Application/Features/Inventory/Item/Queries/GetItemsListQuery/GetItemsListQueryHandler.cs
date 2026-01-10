using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Items.Queries.GetItemsListQuery;

public sealed class GetItemsListQueryHandler
    : IRequestHandler<GetItemsListQuery, Result<List<ItemDto>>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetItemsListQueryHandler> _logger;

    public GetItemsListQueryHandler(IAppDbContext dbContext, ILogger<GetItemsListQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<List<ItemDto>>> Handle(GetItemsListQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching all items from database...");

        var items = await _dbContext.Items.ToListAsync(cancellationToken);

        if (items is null || !items.Any())
        {
            _logger.LogWarning("No items found in inventory.");
            return new List<ItemDto>(); // قائمة فارغة بدون خطأ
        }

        var itemDtos = items.ToDtoList();

        _logger.LogInformation("Retrieved {Count} items successfully.", itemDtos.Count);

        return itemDtos;
    }
}
