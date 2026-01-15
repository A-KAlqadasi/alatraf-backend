using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Items.Queries.GetItemUnitsByItemIdQuery;

public sealed class GetItemUnitsByItemIdQueryHandler
    : IRequestHandler<GetItemUnitsByItemIdQuery, Result<List<ItemUnitDto>>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetItemUnitsByItemIdQueryHandler> _logger;

    public GetItemUnitsByItemIdQueryHandler(
        IAppDbContext dbContext,
        ILogger<GetItemUnitsByItemIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<List<ItemUnitDto>>> Handle(
        GetItemUnitsByItemIdQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching units for item with ID: {ItemId}", request.ItemId);

        var item = await _dbContext.Items
            .AsNoTracking()
            .Include(i => i.ItemUnits).ThenInclude(u => u.Unit)
            .SingleOrDefaultAsync(i => i.Id == request.ItemId, cancellationToken);

        if (item is null)
        {
            _logger.LogWarning("Item with ID {ItemId} not found.", request.ItemId);
            return ItemErrors.NotFound;
        }

        var units = item.ItemUnits.Select(u => new ItemUnitDto
        {
            UnitId = u.UnitId,
            Name = u.Unit.Name,
            Price = u.Price,
            ConversionFactor = u.ConversionFactor,
            MinPriceToPay = u.MinPriceToPay,
            MaxPriceToPay = u.MaxPriceToPay
        }).ToList();

        _logger.LogInformation("Retrieved {Count} units for item {ItemId}.", units.Count, request.ItemId);

        return units;
    }
}
