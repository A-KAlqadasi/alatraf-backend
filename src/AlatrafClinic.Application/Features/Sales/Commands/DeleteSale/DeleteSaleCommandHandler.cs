using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Sales;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Sales.Commands.DeleteSale;

public class DeleteSaleCommandHandler : IRequestHandler<DeleteSaleCommand, Result<Updated>>
{
    private readonly ILogger<DeleteSaleCommandHandler> _logger;
    private readonly IAppDbContext _dbContext;

    public DeleteSaleCommandHandler(ILogger<DeleteSaleCommandHandler> logger, IAppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    public async Task<Result<Updated>> Handle(DeleteSaleCommand command, CancellationToken ct)
    {
        var sale = await _dbContext.Sales
            .Include(s => s.SaleItems)
            .Include(s => s.Diagnosis)
            .SingleOrDefaultAsync(s => s.Id == command.SaleId, ct);
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
        _dbContext.Sales.Remove(sale);
        await _dbContext.SaveChangesAsync(ct);
        _logger.LogInformation("Sale with ID {SaleId} deleted successfully.", command.SaleId);
        return Result.Updated;
    }
}