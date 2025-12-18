using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Sales;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Sales.Commands.DeleteSale;

public class DeleteSaleCommandHandler : IRequestHandler<DeleteSaleCommand, Result<Updated>>
{
    private readonly ILogger<DeleteSaleCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSaleCommandHandler(ILogger<DeleteSaleCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<Updated>> Handle(DeleteSaleCommand command, CancellationToken ct)
    {
        var sale = await _unitOfWork.Sales.GetByIdAsync(command.SaleId, ct);
        if (sale is null)
        {
            _logger.LogError("Sale with ID {SaleId} not found.", command.SaleId);
            return SaleErrors.SaleNotFound;
        }
        var result = sale.Cancel();
        if (result.IsError)
        {
            _logger.LogError("Failed to cancel sale with ID {SaleId}. Error: {Error}", command.SaleId, result.TopError);
            return result;
        }
        await _unitOfWork.Sales.DeleteAsync(sale, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        _logger.LogInformation("Sale with ID {SaleId} deleted successfully.", command.SaleId);
        return Result.Updated;
    }
}