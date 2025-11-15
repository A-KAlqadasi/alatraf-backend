using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Application.Features.Sales.Dtos;
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
        var salesQuery = await _unitOfWork.Sales.GetSalesQueryAsync();

        salesQuery = ApplyFilters(salesQuery, query);

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            salesQuery = ApplySearch(salesQuery, query.SearchTerm!);

        salesQuery = ApplySorting(salesQuery, query.SortColumn, query.SortDirection);

        // Paging
        var page = query.Page < 1 ? 1 : query.Page;
        var size = query.PageSize < 1 ? 10 : query.PageSize;

        var count = await salesQuery.CountAsync(ct);

        var items = await salesQuery
            .Skip((page - 1) * size)
            .Take(size)
            .Select(s => new SaleDto
            {
                SaleId = s.Id,
                SaleStatus = s.Status,
                SaleDate = s.CreatedAtUtc.DateTime.Date,
                Total = s.Total,
                Diagnosis = s.Diagnosis != null ? s.Diagnosis.ToDto() : new DiagnosisDto(),
                SaleItems = s.SaleItems.ToDtos()
            })
            .ToListAsync(ct);

        return new PaginatedList<SaleDto>
        {
            Items = items,
            PageNumber = page,
            PageSize = size,
            TotalCount = count,
            TotalPages = (int)Math.Ceiling(count / (double)size)
        };
    }

    private static IQueryable<Sale> ApplyFilters(IQueryable<Sale> query, GetSalesQuery q)
    {
        if (q.Status.HasValue)
            query = query.Where(s => s.Status == q.Status.Value);

        if (q.DiagnosisId.HasValue && q.DiagnosisId > 0)
            query = query.Where(s => s.DiagnosisId == q.DiagnosisId);

        if (q.PatientId.HasValue && q.PatientId > 0)
            query = query.Where(s => s.Diagnosis.PatientId == q.PatientId);

        if (q.FromDate.HasValue)
            query = query.Where(s => s.CreatedAtUtc >= q.FromDate.Value);

        if (q.ToDate.HasValue)
            query = query.Where(s => s.CreatedAtUtc <= q.ToDate.Value);

        return query;
    }

    private static IQueryable<Sale> ApplySearch(IQueryable<Sale> query, string term)
    {
        var pattern = $"%{term.Trim().ToLower()}%";

        return query.Where(s =>
            (s.Diagnosis != null && EF.Functions.Like(s.Diagnosis.DiagnosisText.ToLower(), pattern)) ||
            s.SaleItems.Any(si => EF.Functions.Like(si.ItemUnit.Item!.Name.ToLower(), pattern)) ||
            (s.Diagnosis != null && s.Diagnosis.Patient != null &&
            EF.Functions.Like(s.Diagnosis.Patient.Person!.FullName.ToLower(), pattern))
        );
    }

    private static IQueryable<Sale> ApplySorting(IQueryable<Sale> query, string sortColumn, string sortDirection)
    {
        var col = sortColumn?.Trim().ToLowerInvariant() ?? "saledate";
        var isDesc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "saledate" => isDesc
                ? query.OrderByDescending(s => s.CreatedAtUtc)
                : query.OrderBy(s => s.CreatedAtUtc),

            "status" => isDesc
                ? query.OrderByDescending(s => s.Status)
                : query.OrderBy(s => s.Status),

            "patient" => isDesc
                ? query.OrderByDescending(s => s.Diagnosis.Patient!.Person!.FullName)
                : query.OrderBy(s => s.Diagnosis.Patient!.Person!.FullName),

            "total" => isDesc
                ? query.OrderByDescending(s => s.Total)
                : query.OrderBy(s => s.Total),

            _ => query.OrderByDescending(s => s.CreatedAtUtc)
        };
    }
}
