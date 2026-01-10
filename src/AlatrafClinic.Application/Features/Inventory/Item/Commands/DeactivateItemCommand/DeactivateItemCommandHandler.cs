using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Items.Commands.DeactivateItemCommand;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class DeactivateItemCommandHandler : IRequestHandler<DeactivateItemCommand, Result<ItemDto>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<DeactivateItemCommandHandler> _logger;

    public DeactivateItemCommandHandler(IAppDbContext dbContext, ILogger<DeactivateItemCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<ItemDto>> Handle(DeactivateItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _dbContext.Items.SingleOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
        if (item == null)
            return ItemErrors.NotFound;
        // var stockExists = await _unitOfWork.Store.HasStockAsync(item.Id, cancellationToken);
        // if (stockExists)
        //     return ItemErrors.CannotDeactivateWithExistingStock;
        // var result = item.Deactivate(stockExists);
        // if (result.IsError)
        //     return result.Errors;

        _dbContext.Items.Update(item);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            IsActive = item.IsActive,
            BaseUnitId = item.BaseUnitId,
            BaseUnitName = item.BaseUnit.Name
        };
    }
}
