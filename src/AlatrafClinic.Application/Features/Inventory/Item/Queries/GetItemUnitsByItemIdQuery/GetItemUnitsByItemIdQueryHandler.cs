using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Items.Queries.GetItemUnitsByItemIdQuery;

public sealed class GetItemUnitsByItemIdQueryHandler
    : IRequestHandler<GetItemUnitsByItemIdQuery, Result<List<ItemUnitDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetItemUnitsByItemIdQueryHandler> _logger;

    public GetItemUnitsByItemIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetItemUnitsByItemIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<List<ItemUnitDto>>> Handle(
        GetItemUnitsByItemIdQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching units for item with ID: {ItemId}", request.ItemId);

        var item = await _unitOfWork.Items.GetByIdWithUnitsAsync(request.ItemId, cancellationToken);
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
