
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Sales.Dtos;
using AlatrafClinic.Application.Features.Sales.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Sales;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Sales.Queries.GetSaleById;

public class GetSaleByIdQueryHandler : IRequestHandler<GetSaleByIdQuery, Result<SaleDto>>
{
    private readonly ILogger<GetSaleByIdQueryHandler> _logger;
    private readonly IAppDbContext _dbContext;

    public GetSaleByIdQueryHandler(ILogger<GetSaleByIdQueryHandler> logger, IAppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    public async Task<Result<SaleDto>> Handle(GetSaleByIdQuery query, CancellationToken ct)
    {
        var sale = await _dbContext.Sales
            .Include(s => s.SaleItems)
            .Include(s => s.Diagnosis)
            .SingleOrDefaultAsync(s => s.Id == query.SaleId, ct);
        if (sale is null)
        {
            _logger.LogError("Sale {saleId} is not found", query.SaleId);
            return SaleErrors.SaleNotFound;
        }

        return sale.ToDto();
    }
}