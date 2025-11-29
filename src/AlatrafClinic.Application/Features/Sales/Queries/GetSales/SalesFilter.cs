using AlatrafClinic.Application.Common.Specifications;
using AlatrafClinic.Domain.Sales;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Sales.Queries.GetSales;

public sealed class SalesFilter : FilterSpecification<Sale>
{
    private readonly GetSalesQuery _q;

    public SalesFilter(GetSalesQuery q)
        : base(q.Page, q.PageSize)
    {
        _q = q;
    }

    public override IQueryable<Sale> Apply(IQueryable<Sale> query)
    {
        query = query
            .Include(s => s.Diagnosis)
                .ThenInclude(d => d.Patient)
                    .ThenInclude(p => p.Person)
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.ItemUnit)
                    .ThenInclude(iu => iu.Item)
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.ItemUnit)
                    .ThenInclude(u=> u.Unit);

        query = ApplyFilters(query);
        query = ApplySearch(query);
        query = ApplySorting(query);

        return query;
    }

    // ------------- FILTERS -------------
    private IQueryable<Sale> ApplyFilters(IQueryable<Sale> query)
    {
        if (_q.Status.HasValue)
            query = query.Where(s => s.Status == _q.Status.Value);

        if (_q.DiagnosisId.HasValue && _q.DiagnosisId.Value > 0)
            query = query.Where(s => s.DiagnosisId == _q.DiagnosisId.Value);

        if (_q.PatientId.HasValue && _q.PatientId.Value > 0)
            query = query.Where(s =>
                s.Diagnosis != null &&
                s.Diagnosis.PatientId == _q.PatientId.Value);

        if (_q.FromDate.HasValue)
            query = query.Where(s => s.CreatedAtUtc >= _q.FromDate.Value);

        if (_q.ToDate.HasValue)
            query = query.Where(s => s.CreatedAtUtc <= _q.ToDate.Value);

        return query;
    }

    // ------------- SEARCH -------------
    private IQueryable<Sale> ApplySearch(IQueryable<Sale> query)
    {
        if (string.IsNullOrWhiteSpace(_q.SearchTerm))
            return query;

        var pattern = $"%{_q.SearchTerm!.Trim().ToLower()}%";

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

    // ------------- SORTING -------------
    private IQueryable<Sale> ApplySorting(IQueryable<Sale> query)
    {
        var col = _q.SortColumn?.Trim().ToLowerInvariant() ?? "saledate";
        var isDesc = string.Equals(_q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

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
