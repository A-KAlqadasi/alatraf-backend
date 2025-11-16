using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.Items.Commands.DeleteItemCommand;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;

using MediatR;
using Microsoft.Extensions.Logging;


public class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand, Result<Deleted>>
{
    private readonly ILogger<DeleteItemCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteItemCommandHandler(ILogger<DeleteItemCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Deleted>> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Items.GetByIdAsync(request.Id, cancellationToken);
        if (item is null)
            return ItemErrors.NotFound;
        // if( await _unitOfWork.Stocks.HasStockAsync(item.Id, cancellationToken))
        //     return ItemErrors.CannotDeleteWithExistingStock;

        await _unitOfWork.Items.DeleteAsync(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted item with Id={ItemId}", item.Id);
        return Result.Deleted;
    }
}
