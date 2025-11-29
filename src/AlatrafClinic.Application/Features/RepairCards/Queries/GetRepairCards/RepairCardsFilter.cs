using AlatrafClinic.Application.Common.Specifications;
using AlatrafClinic.Domain.RepairCards;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetRepairCards;

public sealed class RepairCardsFilter : FilterSpecification<RepairCard>
{
    private readonly GetRepairCardsQuery _q;

    public RepairCardsFilter(GetRepairCardsQuery q)
        : base(q.Page, q.PageSize)
    {
        _q = q;
    }

    public override IQueryable<RepairCard> Apply(IQueryable<RepairCard> query)
    {
        query = query
            .Include(rc => rc.Diagnosis)
                .ThenInclude(d => d.Patient)
                    .ThenInclude(p => p.Person)
            .Include(rc => rc.DiagnosisIndustrialParts)
                .ThenInclude(dip => dip.IndustrialPartUnit)
                    .ThenInclude(ipu => ipu.IndustrialPart)
            .Include(rc => rc.DiagnosisIndustrialParts)
                .ThenInclude(dip => dip.IndustrialPartUnit)
                    .ThenInclude(ipu => ipu.Unit)
            .Include(rc => rc.DeliveryTime);

        query = ApplyFilters(query);
        query = ApplySearch(query);
        query = ApplySorting(query);

        return query;
    }

    // ---------------- FILTERS ----------------
    private IQueryable<RepairCard> ApplyFilters(IQueryable<RepairCard> query)
    {
        if (_q.IsActive.HasValue)
            query = query.Where(rc => rc.IsActive == _q.IsActive.Value);

        if (_q.IsLate.HasValue)
            query = query.Where(rc => rc.IsLate == _q.IsLate.Value);

        if (_q.Status.HasValue)
            query = query.Where(rc => rc.Status == _q.Status.Value);

        if (_q.DiagnosisId.HasValue && _q.DiagnosisId.Value > 0)
            query = query.Where(rc => rc.DiagnosisId == _q.DiagnosisId.Value);

        if (_q.PatientId.HasValue && _q.PatientId.Value > 0)
            query = query.Where(rc =>
                rc.Diagnosis != null &&
                rc.Diagnosis.PatientId == _q.PatientId.Value);

        return query;
    }

    // ---------------- SEARCH ----------------
    private IQueryable<RepairCard> ApplySearch(IQueryable<RepairCard> query)
    {
        if (string.IsNullOrWhiteSpace(_q.SearchTerm))
            return query;

        var pattern = $"%{_q.SearchTerm!.Trim().ToLower()}%";

        return query.Where(rc =>
            (rc.Diagnosis != null &&
                (EF.Functions.Like(rc.Diagnosis.DiagnosisText.ToLower(), pattern) ||
                 (rc.Diagnosis.Patient != null &&
                  rc.Diagnosis.Patient.Person != null &&
                  EF.Functions.Like(rc.Diagnosis.Patient.Person.FullName.ToLower(), pattern))))
            ||
            rc.DiagnosisIndustrialParts.Any(p =>
                p.IndustrialPartUnit != null &&
                p.IndustrialPartUnit.IndustrialPart != null &&
                EF.Functions.Like(p.IndustrialPartUnit.IndustrialPart.Name.ToLower(), pattern))
        );
    }

    // ---------------- SORTING ----------------
    private IQueryable<RepairCard> ApplySorting(IQueryable<RepairCard> query)
    {
        var col = _q.SortColumn?.Trim().ToLowerInvariant() ?? "deliverydate";
        var isDesc = string.Equals(_q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "deliverydate" => isDesc
                ? query.OrderByDescending(rc => rc.DeliveryTime!.DeliveryDate)
                : query.OrderBy(rc => rc.DeliveryTime!.DeliveryDate),

            "status" => isDesc
                ? query.OrderByDescending(rc => rc.Status)
                : query.OrderBy(rc => rc.Status),

            "patient" => isDesc
                ? query.OrderByDescending(rc => rc.Diagnosis!.Patient!.Person!.FullName)
                : query.OrderBy(rc => rc.Diagnosis!.Patient!.Person!.FullName),

            _ => query.OrderByDescending(rc => rc.CreatedAtUtc)
        };
    }
}
