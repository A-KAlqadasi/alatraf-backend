using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Application.Features.Sales.Dtos;
using AlatrafClinic.Application.Features.Sales.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Sales;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Sales.Queries.GetSales;

public sealed class GetSalesQueryHandler
    : IRequestHandler<GetSalesQuery, Result<PaginatedList<SaleDto>>>
{
    private readonly IAppDbContext _context;

    public GetSalesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<SaleDto>>> Handle(
        GetSalesQuery query,
        CancellationToken ct)
    {
        IQueryable<Sale> salesQuery = _context.Sales
            .Include(s => s.Diagnosis)
                .ThenInclude(d => d.Patient)
                    .ThenInclude(p => p.Person)
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.ItemUnit)
                    .ThenInclude(iu => iu.Item)
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.ItemUnit)
                    .ThenInclude(iu => iu.Unit)
            .AsNoTracking();

        if(string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            salesQuery = ApplySearch(salesQuery, query);
        }
        
        salesQuery = ApplyFilters(salesQuery, query);
        salesQuery = ApplySorting(salesQuery, query);

        var totalCount = await salesQuery.CountAsync(ct);

        var skip = (query.Page - 1) * query.PageSize;

        var sales = await salesQuery
            .Skip(skip)
            .Take(query.PageSize)
            .ToListAsync(ct);

        var items = sales
            .Select(s => s.ToDto())
            .ToList();

        return new PaginatedList<SaleDto>
        {
            Items      = items,
            PageNumber = query.Page,
            PageSize   = query.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
        };
    }

    private static IQueryable<Sale> ApplyFilters(
        IQueryable<Sale> query,
        GetSalesQuery q)
    {
        if (q.Status.HasValue)
            query = query.Where(s => s.Status == q.Status.Value);

        if (q.DiagnosisId.HasValue && q.DiagnosisId.Value > 0)
            query = query.Where(s => s.DiagnosisId == q.DiagnosisId.Value);

        if (q.PatientId.HasValue && q.PatientId.Value > 0)
            query = query.Where(s =>
                s.Diagnosis != null &&
                s.Diagnosis.PatientId == q.PatientId.Value);

        if (q.FromDate.HasValue)
            query = query.Where(s => DateOnly.FromDateTime(s.CreatedAtUtc.DateTime) >= q.FromDate.Value);

        if (q.ToDate.HasValue)
            query = query.Where(s => DateOnly.FromDateTime(s.CreatedAtUtc.DateTime) <= q.ToDate.Value);

        return query;
    }

    private static IQueryable<Sale> ApplySearch(
        IQueryable<Sale> query,
        GetSalesQuery q)
    {
        if (string.IsNullOrWhiteSpace(q.SearchTerm))
            return query;

        var pattern = $"%{q.SearchTerm!.Trim().ToLower()}%";

        return query.Where(s =>
            (s.Diagnosis != null &&
             EF.Functions.Like(s.Diagnosis.DiagnosisText.ToLower(), pattern))
            ||
            s.SaleItems.Any(si =>
                si.ItemUnit != null &&
                si.ItemUnit.Item != null &&
                EF.Functions.Like(si.ItemUnit.Item.Name.ToLower(), pattern))
            ||
            (s.Diagnosis != null &&
             s.Diagnosis.Patient != null &&
             s.Diagnosis.Patient.Person != null &&
             EF.Functions.Like(s.Diagnosis.Patient.Person.FullName.ToLower(), pattern))
        );
    }

    private static IQueryable<Sale> ApplySorting(
        IQueryable<Sale> query,
        GetSalesQuery q)
    {
        var col    = q.SortColumn?.Trim().ToLowerInvariant() ?? "saledate";
        var isDesc = string.Equals(q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "saledate" => isDesc
                ? query.OrderByDescending(s => s.CreatedAtUtc)
                : query.OrderBy(s => s.CreatedAtUtc),

            "status" => isDesc
                ? query.OrderByDescending(s => s.Status)
                : query.OrderBy(s => s.Status),

            "patient" => isDesc
                ? query.OrderByDescending(s => s.Diagnosis!.Patient!.Person!.FullName)
                : query.OrderBy(s => s.Diagnosis!.Patient!.Person!.FullName),

            "total" => isDesc
                ? query.OrderByDescending(s => s.Total)
                : query.OrderBy(s => s.Total),

            _ => query.OrderByDescending(s => s.CreatedAtUtc)
        };
    }
}
