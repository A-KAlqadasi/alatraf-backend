using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Items.Commands.CreateItemCommand;

public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, Result<ItemDto>>
{
    private readonly HybridCache _cache;
    private readonly ILogger<CreateItemCommandHandler> _logger;
    private readonly IAppDbContext _dbContext;

    public CreateItemCommandHandler(
        HybridCache cache,
        ILogger<CreateItemCommandHandler> logger,
        IAppDbContext dbContext)
    {
        _cache = cache;
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Result<ItemDto>> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {

        var baseUnit = await _dbContext.Units.SingleOrDefaultAsync(u => u.Id == request.BaseUnitId, cancellationToken);
        if (baseUnit is null)
            return ItemErrors.BaseUnitIsRequired;


        var createResult = Domain.Inventory.Items.Item.Create(request.Name, baseUnit, request.Description);
        if (createResult.IsError)
            return createResult.Errors;

        var item = createResult.Value;


        if (request.Units is not null && request.Units.Any())
        {
            foreach (var unitDto in request.Units)
            {
                var addResult = item.AddOrUpdateItemUnit(
                    unitDto.UnitId,
                    unitDto.Price,
                    unitDto.ConversionFactor,
                    unitDto.MinPriceToPay,
                    unitDto.MaxPriceToPay);

                if (addResult.IsError)
                    return addResult.Errors;
            }
        }


        await _dbContext.Items.AddAsync(item, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);


        var itemDto = new ItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            IsActive = item.IsActive,
            BaseUnitId = item.BaseUnitId,
            BaseUnitName = item.BaseUnit.Name!,
            Units = item.ItemUnits.Select(iu => new ItemUnitDto
            {
                UnitId = iu.UnitId,
                Price = iu.Price,
                ConversionFactor = iu.ConversionFactor,
                MinPriceToPay = iu.MinPriceToPay,
                MaxPriceToPay = iu.MaxPriceToPay
            }).ToList()
        };


        await _cache.SetAsync($"item:{item.Id}", itemDto, cancellationToken: cancellationToken);

        _logger.LogInformation("Created new item: {ItemName} (Id={ItemId})", item.Name, item.Id);


        return itemDto;
    }
}
