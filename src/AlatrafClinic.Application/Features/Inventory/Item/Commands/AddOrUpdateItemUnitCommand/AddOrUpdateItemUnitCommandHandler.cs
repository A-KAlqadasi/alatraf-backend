using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.Items.Commands.AddOrUpdateItemUnitCommand;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;
using MediatR;
using Microsoft.Extensions.Logging;

public class AddOrUpdateItemUnitCommandHandler : IRequestHandler<AddOrUpdateItemUnitCommand, Result<Updated>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddOrUpdateItemUnitCommandHandler> _logger;

    public AddOrUpdateItemUnitCommandHandler(IUnitOfWork unitOfWork, ILogger<AddOrUpdateItemUnitCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Updated>> Handle(AddOrUpdateItemUnitCommand request, CancellationToken cancellationToken)
    {
        
        var item = await _unitOfWork.Items.GetByIdAsync(request.ItemId, cancellationToken);
        if (item is null)
        {
            _logger.LogWarning("Item with Id {ItemId} not found.", request.ItemId);
            return ItemErrors.NotFound;
        }

        
        var result = item.AddOrUpdateItemUnit(
            request.UnitId,
            request.Price,
            request.ConversionFactor,
            request.MinPriceToPay,
            request.MaxPriceToPay
        );

        if (result.IsError)
        {
            _logger.LogWarning("Failed to add/update ItemUnit: {Errors}", result.Errors);
            return result.Errors;
        }

        
        await _unitOfWork.Items.UpdateAsync(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("ItemUnit for Item {ItemId} updated successfully.", request.ItemId);

        return Result.Updated;
    }
}
