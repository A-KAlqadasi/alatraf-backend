using AlatrafClinic.Application.Common.Specifications;
using AlatrafClinic.Domain.Sessions;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetSessions;

public sealed class SessionsFilter : FilterSpecification<Session>
{
    private readonly GetSessionsQuery _q;

    public SessionsFilter(GetSessionsQuery q)
        : base(q.Page, q.PageSize)
    {
        _q = q;
    }

    public override IQueryable<Session> Apply(IQueryable<Session> query)
    {
        // Includes needed for filters/search/projection
        query = query
            .Include(s => s.TherapyCard)
                .ThenInclude(tc => tc.Diagnosis)
                    .ThenInclude(d => d.Patient)
                        .ThenInclude(p => p.Person)
            .Include(s => s.SessionPrograms)
                .ThenInclude(sp => sp.DiagnosisProgram)
                    .ThenInclude(dp => dp.MedicalProgram)
            .Include(s => s.SessionPrograms)
                .ThenInclude(sp => sp.DoctorSectionRoom)
                    .ThenInclude(dsr => dsr!.Section)
            .Include(s => s.SessionPrograms)
                .ThenInclude(sp => sp.DoctorSectionRoom)
                    .ThenInclude(dsr => dsr!.Room)
            .Include(s => s.SessionPrograms)
                .ThenInclude(sp => sp.DoctorSectionRoom)
                    .ThenInclude(dsr => dsr!.Doctor)
                        .ThenInclude(d => d.Person);

        query = ApplyFilters(query);
        query = ApplySearch(query);
        query = ApplySorting(query);

        return query;
    }

    // ---------------- FILTERS ----------------
    private IQueryable<Session> ApplyFilters(IQueryable<Session> query)
    {
        if (_q.TherapyCardId.HasValue && _q.TherapyCardId.Value > 0)
            query = query.Where(s => s.TherapyCardId == _q.TherapyCardId.Value);

        if (_q.IsTaken.HasValue)
            query = query.Where(s => s.IsTaken == _q.IsTaken.Value);

        if (_q.FromDate.HasValue)
            query = query.Where(s => s.SessionDate >= _q.FromDate.Value);

        if (_q.ToDate.HasValue)
            query = query.Where(s => s.SessionDate <= _q.ToDate.Value);

        if (_q.DoctorId.HasValue && _q.DoctorId.Value > 0)
        {
            var doctorId = _q.DoctorId.Value;
            query = query.Where(s =>
                s.SessionPrograms.Any(sp =>
                    sp.DoctorSectionRoom != null &&
                    sp.DoctorSectionRoom.DoctorId == doctorId));
        }

        if (_q.PatientId.HasValue && _q.PatientId.Value > 0)
        {
            var patientId = _q.PatientId.Value;
            query = query.Where(s =>
                s.TherapyCard != null &&
                s.TherapyCard.Diagnosis != null &&
                s.TherapyCard.Diagnosis.PatientId == patientId);
        }

        return query;
    }

    // ---------------- SEARCH ----------------
    private IQueryable<Session> ApplySearch(IQueryable<Session> query)
    {
        if (string.IsNullOrWhiteSpace(_q.SearchTerm))
            return query;

        var pattern = $"%{_q.SearchTerm!.Trim().ToLower()}%";

        return query.Where(s =>
            (s.TherapyCard != null &&
             s.TherapyCard.Diagnosis != null &&
             s.TherapyCard.Diagnosis.Patient != null &&
             s.TherapyCard.Diagnosis.Patient.Person != null &&
             EF.Functions.Like(s.TherapyCard.Diagnosis.Patient.Person.FullName.ToLower(), pattern))
            ||
            s.SessionPrograms.Any(sp =>
                sp.DiagnosisProgram != null &&
                sp.DiagnosisProgram.MedicalProgram != null &&
                EF.Functions.Like(sp.DiagnosisProgram.MedicalProgram.Name.ToLower(), pattern))
        );
    }

    // ---------------- SORTING ----------------
    private IQueryable<Session> ApplySorting(IQueryable<Session> query)
    {
        var col = _q.SortColumn?.Trim().ToLowerInvariant() ?? "sessiondate";
        var isDesc = string.Equals(_q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "sessiondate" => isDesc
                ? query.OrderByDescending(s => s.SessionDate)
                : query.OrderBy(s => s.SessionDate),

            "number" => isDesc
                ? query.OrderByDescending(s => s.Number)
                : query.OrderBy(s => s.Number),

            "patient" => isDesc
                ? query.OrderByDescending(s => s.TherapyCard!.Diagnosis!.Patient!.Person!.FullName)
                : query.OrderBy(s => s.TherapyCard!.Diagnosis!.Patient!.Person!.FullName),

            _ => query.OrderByDescending(s => s.SessionDate)
        };
    }
}
