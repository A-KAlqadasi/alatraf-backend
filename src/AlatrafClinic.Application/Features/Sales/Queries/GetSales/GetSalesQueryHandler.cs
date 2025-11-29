using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Application.Features.Sales.Dtos;
using AlatrafClinic.Application.Features.Sales.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Sales;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Sales.Queries.GetSales;

public sealed class GetSalesQueryHandler : IRequestHandler<GetSalesQuery, Result<PaginatedList<SaleDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSalesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<SaleDto>>> Handle(GetSalesQuery query, CancellationToken ct)
    {
        var spec = new SalesFilter(query);

        var totalCount = await _unitOfWork.Sales.CountAsync(spec, ct);

        var sales = await _unitOfWork.Sales
            .ListAsync(spec, spec.Page, spec.PageSize, ct);

        var items = sales
            .Select(s => s.ToDto())
            .ToList();

        var paged = new PaginatedList<SaleDto>
        {
            Items      = items,
            PageNumber = spec.Page,
            PageSize   = spec.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)spec.PageSize)
        };

        return paged;
    }
}
