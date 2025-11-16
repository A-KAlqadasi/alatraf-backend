
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Sales.Dtos;
using AlatrafClinic.Application.Features.Sales.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Sales;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Sales.Queries.GetSaleById;

public class GetSaleByIdQueryHandler : IRequestHandler<GetSaleByIdQuery, Result<SaleDto>>
{
    private readonly ILogger<GetSaleByIdQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetSaleByIdQueryHandler(ILogger<GetSaleByIdQueryHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<SaleDto>> Handle(GetSaleByIdQuery query, CancellationToken ct)
    {
        var sale = await _unitOfWork.Sales.GetByIdAsync(query.SaleId, ct);
        if (sale is null)
        {
            _logger.LogError("Sale {saleId} is not found", query.SaleId);
            return SaleErrors.SaleNotFound;
        }

        return sale.ToDto();
    }
}