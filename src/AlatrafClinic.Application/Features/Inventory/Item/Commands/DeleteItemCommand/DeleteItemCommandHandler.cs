using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Items.Commands.DeleteItemCommand;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


public class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand, Result<Deleted>>
{
    private readonly ILogger<DeleteItemCommandHandler> _logger;
    private readonly IAppDbContext _dbContext;

    public DeleteItemCommandHandler(ILogger<DeleteItemCommandHandler> logger, IAppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Result<Deleted>> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _dbContext.Items.SingleOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
        if (item is null)
            return ItemErrors.NotFound;
        if (await _dbContext.StoreItemUnits
            .AsNoTracking()
            .AnyAsync(siu => siu.ItemUnit.ItemId == item.Id && siu.Quantity > 0, cancellationToken))
            return ItemErrors.CannotDeleteWithExistingStock;

        _dbContext.Items.Remove(item);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted item with Id={ItemId}", item.Id);
        return Result.Deleted;
    }
}
