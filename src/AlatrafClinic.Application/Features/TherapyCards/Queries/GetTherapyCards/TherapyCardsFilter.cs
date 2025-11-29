using AlatrafClinic.Application.Common.Specifications;
using AlatrafClinic.Domain.TherapyCards;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetTherapyCards;

public sealed class TherapyCardsFilter : FilterSpecification<TherapyCard>
{
    private readonly GetTherapyCardsQuery _q;

    public TherapyCardsFilter(GetTherapyCardsQuery q)
        : base(q.Page, q.PageSize)
    {
        _q = q;
    }

    public override IQueryable<TherapyCard> Apply(IQueryable<TherapyCard> query)
    {
        // Includes needed for filters/search/projection
        query = query
            .Include(tc => tc.Diagnosis)
                .ThenInclude(d => d.Patient)
                    .ThenInclude(p => p.Person)
            .Include(tc => tc.DiagnosisPrograms)
                .ThenInclude(dp => dp.MedicalProgram)
            .Include(tc => tc.Sessions);

        query = ApplyFilters(query);
        query = ApplySearch(query);
        query = ApplySorting(query);

        return query;
    }

    // ---------------- FILTERS ----------------
    private IQueryable<TherapyCard> ApplyFilters(IQueryable<TherapyCard> query)
    {
        if (_q.IsActive.HasValue)
            query = query.Where(tc => tc.IsActive == _q.IsActive.Value);

        if (_q.Type.HasValue)
            query = query.Where(tc => tc.Type == _q.Type.Value);

        if (_q.Status.HasValue)
            query = query.Where(tc => tc.CardStatus == _q.Status.Value);

        if (_q.ProgramStartFrom.HasValue)
            query = query.Where(tc => tc.ProgramStartDate >= _q.ProgramStartFrom.Value);

        if (_q.ProgramStartTo.HasValue)
            query = query.Where(tc => tc.ProgramStartDate <= _q.ProgramStartTo.Value);

        if (_q.ProgramEndFrom.HasValue)
            query = query.Where(tc => tc.ProgramEndDate >= _q.ProgramEndFrom.Value);

        if (_q.ProgramEndTo.HasValue)
            query = query.Where(tc => tc.ProgramEndDate <= _q.ProgramEndTo.Value);

        if (_q.DiagnosisId.HasValue && _q.DiagnosisId.Value > 0)
            query = query.Where(tc => tc.DiagnosisId == _q.DiagnosisId.Value);

        if (_q.PatientId.HasValue && _q.PatientId.Value > 0)
            query = query.Where(tc =>
                tc.Diagnosis != null &&
                tc.Diagnosis.PatientId == _q.PatientId.Value);

        return query;
    }

    // ---------------- SEARCH ----------------
    private IQueryable<TherapyCard> ApplySearch(IQueryable<TherapyCard> query)
    {
        if (string.IsNullOrWhiteSpace(_q.SearchTerm))
            return query;

        var pattern = $"%{_q.SearchTerm!.Trim().ToLower()}%";

        return query.Where(tc =>
            (tc.Diagnosis != null &&
                (EF.Functions.Like(tc.Diagnosis.DiagnosisText.ToLower(), pattern) ||
                 (tc.Diagnosis.Patient != null &&
                  tc.Diagnosis.Patient.Person != null &&
                  EF.Functions.Like(tc.Diagnosis.Patient.Person.FullName.ToLower(), pattern))))
            ||
            tc.DiagnosisPrograms.Any(dp =>
                dp.MedicalProgram != null &&
                EF.Functions.Like(dp.MedicalProgram.Name.ToLower(), pattern))
        );
    }

    // ---------------- SORTING ----------------
    private IQueryable<TherapyCard> ApplySorting(IQueryable<TherapyCard> query)
    {
        var col = _q.SortColumn?.Trim().ToLowerInvariant() ?? "programstartdate";
        var isDesc = string.Equals(_q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "programstartdate" => isDesc
                ? query.OrderByDescending(tc => tc.ProgramStartDate)
                : query.OrderBy(tc => tc.ProgramStartDate),

            "programenddate" => isDesc
                ? query.OrderByDescending(tc => tc.ProgramEndDate)
                : query.OrderBy(tc => tc.ProgramEndDate),

            "type" => isDesc
                ? query.OrderByDescending(tc => tc.Type)
                : query.OrderBy(tc => tc.Type),

            "status" => isDesc
                ? query.OrderByDescending(tc => tc.CardStatus)
                : query.OrderBy(tc => tc.CardStatus),

            "patient" => isDesc
                ? query.OrderByDescending(tc => tc.Diagnosis!.Patient!.Person!.FullName)
                : query.OrderBy(tc => tc.Diagnosis!.Patient!.Person!.FullName),

            _ => query.OrderByDescending(tc => tc.ProgramStartDate)
        };
    }
}
