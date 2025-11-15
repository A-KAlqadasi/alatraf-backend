using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Sales;

using MediatR;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Sales.Commands.CancelSale;

public class CancelSaleCommandHandler : IRequestHandler<CancelSaleCommand, Result<Updated>>
{
    private readonly ILogger<CancelSaleCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly HybridCache _cache;

    public CancelSaleCommandHandler(ILogger<CancelSaleCommandHandler> logger, IUnitOfWork unitOfWork, HybridCache cache)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }
    public async Task<Result<Updated>> Handle(CancelSaleCommand command, CancellationToken ct)
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
        await _unitOfWork.Sales.UpdateAsync(sale, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result.Updated;
    }
}