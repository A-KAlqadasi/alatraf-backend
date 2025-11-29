using AlatrafClinic.Application.Common.Specifications;
using AlatrafClinic.Domain.Diagnosises;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Diagnosises.Queries.GetDiagnoses;

public sealed class DiagnosesFilter : FilterSpecification<Diagnosis>
{
    private readonly GetDiagnosesQuery _q;

    public DiagnosesFilter(GetDiagnosesQuery q)
        : base(q.Page, q.PageSize)
    {
        _q = q;
    }

    public override IQueryable<Diagnosis> Apply(IQueryable<Diagnosis> query)
    {
        // includes (only what you really need for filters/sort/search)
        query = query
            .Include(d => d.Patient)
                .ThenInclude(p => p.Person)
            .Include(d => d.Ticket)
            .Include(d => d.DiagnosisPrograms)
                .ThenInclude(dp => dp.MedicalProgram)
            .Include(d => d.DiagnosisIndustrialParts)
                .ThenInclude(di => di.IndustrialPartUnit)
                    .ThenInclude(u => u.IndustrialPart)
            .Include(d => d.RepairCard)
            .Include(d => d.Sale)
            .Include(d => d.TherapyCard);

        query = ApplyFilters(query);
        query = ApplySearch(query);
        query = ApplySorting(query);

        return query;
    }

    // -------------------- FILTERS --------------------
    private IQueryable<Diagnosis> ApplyFilters(IQueryable<Diagnosis> query)
    {
        if (_q.Type.HasValue)
            query = query.Where(d => d.DiagnoType == _q.Type.Value);

        if (_q.PatientId.HasValue && _q.PatientId.Value > 0)
            query = query.Where(d => d.PatientId == _q.PatientId.Value);

        if (_q.TicketId.HasValue && _q.TicketId.Value > 0)
            query = query.Where(d => d.TicketId == _q.TicketId.Value);

        if (_q.HasRepairCard.HasValue)
            query = _q.HasRepairCard.Value
                ? query.Where(d => d.RepairCard != null)
                : query.Where(d => d.RepairCard == null);

        if (_q.HasSale.HasValue)
            query = _q.HasSale.Value
                ? query.Where(d => d.Sale != null)
                : query.Where(d => d.Sale == null);

        if (_q.HasTherapyCards.HasValue)
            query = _q.HasTherapyCards.Value
                ? query.Where(d => d.TherapyCard != null)
                : query.Where(d => d.TherapyCard == null);

        if (_q.InjuryDateFrom.HasValue)
            query = query.Where(d => d.InjuryDate >= _q.InjuryDateFrom.Value);

        if (_q.InjuryDateTo.HasValue)
            query = query.Where(d => d.InjuryDate <= _q.InjuryDateTo.Value);

        if (_q.CreatedDateFrom.HasValue)
        {
            var fromUtc = DateTime.SpecifyKind(_q.CreatedDateFrom.Value, DateTimeKind.Utc);
            query = query.Where(d => d.CreatedAtUtc >= fromUtc);
        }

        if (_q.CreatedDateTo.HasValue)
        {
            var toUtc = DateTime.SpecifyKind(_q.CreatedDateTo.Value, DateTimeKind.Utc);
            query = query.Where(d => d.CreatedAtUtc <= toUtc);
        }

        return query;
    }

    // -------------------- SEARCH --------------------
    private IQueryable<Diagnosis> ApplySearch(IQueryable<Diagnosis> query)
    {
        if (string.IsNullOrWhiteSpace(_q.SearchTerm))
            return query;

        var searchTerm = _q.SearchTerm!;
        var pattern = $"%{searchTerm.Trim().ToLower()}%";

        return query.Where(d =>
            EF.Functions.Like(d.DiagnosisText.ToLower(), pattern) ||
            EF.Functions.Like(d.Id.ToString(), pattern) ||
            (d.Patient != null && d.Patient.Person != null &&
                EF.Functions.Like(d.Patient.Person.FullName.ToLower(), pattern)) ||
            (d.Ticket != null && EF.Functions.Like(d.Ticket.Id.ToString(), pattern)) ||
            d.DiagnosisPrograms.Any(dp =>
                dp.MedicalProgram != null &&
                EF.Functions.Like(dp.MedicalProgram.Name.ToLower(), pattern)) ||
            d.DiagnosisIndustrialParts.Any(di =>
                di.IndustrialPartUnit != null &&
                di.IndustrialPartUnit.IndustrialPart != null &&
                EF.Functions.Like(di.IndustrialPartUnit.IndustrialPart.Name.ToLower(), pattern))
        );
    }

    // -------------------- SORTING --------------------
    private IQueryable<Diagnosis> ApplySorting(IQueryable<Diagnosis> query)
    {
        var col = _q.SortColumn?.Trim().ToLowerInvariant() ?? "createdat";
        var isDesc = string.Equals(_q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "createdat"  => isDesc
                ? query.OrderByDescending(d => d.CreatedAtUtc)
                : query.OrderBy(d => d.CreatedAtUtc),

            "injurydate" => isDesc
                ? query.OrderByDescending(d => d.InjuryDate)
                : query.OrderBy(d => d.InjuryDate),

            "type"       => isDesc
                ? query.OrderByDescending(d => d.DiagnoType)
                : query.OrderBy(d => d.DiagnoType),

            "patient"    => isDesc
                ? query.OrderByDescending(d =>
                    d.Patient != null && d.Patient.Person != null
                        ? d.Patient.Person.FullName
                        : string.Empty)
                : query.OrderBy(d =>
                    d.Patient != null && d.Patient.Person != null
                        ? d.Patient.Person.FullName
                        : string.Empty),

            "ticket"     => isDesc
                ? query.OrderByDescending(d => d.Ticket != null ? d.Ticket.Id : 0)
                : query.OrderBy(d => d.Ticket != null ? d.Ticket.Id : 0),

            _            => query.OrderByDescending(d => d.CreatedAtUtc)
        };
    }
}
