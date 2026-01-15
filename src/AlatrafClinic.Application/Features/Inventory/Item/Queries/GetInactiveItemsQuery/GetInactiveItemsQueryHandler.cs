using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Items.Queries.GetInactiveItemsQuery;

public sealed class GetInactiveItemsQueryHandler : IRequestHandler<GetInactiveItemsQuery, Result<List<ItemDto>>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetInactiveItemsQueryHandler> _logger;

    public GetInactiveItemsQueryHandler(IAppDbContext dbContext, ILogger<GetInactiveItemsQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<List<ItemDto>>> Handle(GetInactiveItemsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching inactive items...");

        var inactiveItems = await _dbContext.Items
            .AsNoTracking()
            .Where(i => !i.IsActive)
            .Include(i => i.BaseUnit)
            .Include(i => i.ItemUnits).ThenInclude(u => u.Unit)
            .ToListAsync(cancellationToken);

        if (inactiveItems.Count == 0)
        {
            _logger.LogWarning("No inactive items found.");
            return new List<ItemDto>(); // إرجاع قائمة فارغة بدون خطأ
        }

        var itemDtos = inactiveItems.ToDtoList();
        _logger.LogInformation("Retrieved {Count} inactive items successfully.", itemDtos.Count);

        return itemDtos;
    }
}
