using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Items.Queries.GetItemsWithUnitsQuery;

public sealed class GetItemsWithUnitsQueryHandler : IRequestHandler<GetItemsWithUnitsQuery, Result<List<ItemDto>>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetItemsWithUnitsQueryHandler> _logger;

    public GetItemsWithUnitsQueryHandler(IAppDbContext dbContext, ILogger<GetItemsWithUnitsQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<List<ItemDto>>> Handle(GetItemsWithUnitsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching all items with their units...");

        var items = await _dbContext.Items
            .AsNoTracking()
            .Include(i => i.BaseUnit)
            .Include(i => i.ItemUnits).ThenInclude(u => u.Unit)
            .ToListAsync(cancellationToken);

        if (items.Count == 0)
        {
            _logger.LogWarning("No items found in the system.");
            return new List<ItemDto>(); // لا نرجع خطأ، فقط قائمة فارغة
        }

        var itemDtos = items.ToDtoList();

        _logger.LogInformation("Retrieved {Count} items with units successfully.", itemDtos.Count);

        return itemDtos;
    }
}
