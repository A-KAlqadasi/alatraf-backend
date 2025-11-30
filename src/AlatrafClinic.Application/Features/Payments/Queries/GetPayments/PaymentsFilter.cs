using AlatrafClinic.Application.Common.Specifications;
using AlatrafClinic.Domain.Payments;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Payments.Queries.GetPayments;

public sealed class PaymentsFilter : FilterSpecification<Payment>
{
    private readonly GetPaymentsQuery _q;

    public PaymentsFilter(GetPaymentsQuery q)
        : base(q.Page, q.PageSize)
    {
        _q = q;
    }

    public override IQueryable<Payment> Apply(IQueryable<Payment> query)
    {
        // Includes needed for filters/search/sorting
        query = query
            .Include(p => p.Diagnosis)
                .ThenInclude(d => d.Patient)
                    .ThenInclude(pat => pat.Person);

        query = ApplyFilters(query);
        query = ApplySearch(query);
        query = ApplySorting(query);

        return query;
    }

    // ---------------- FILTERS ----------------
    private IQueryable<Payment> ApplyFilters(IQueryable<Payment> query)
    {
        if (_q.AccountKind.HasValue)
            query = query.Where(p => p.AccountKind == _q.AccountKind.Value);

        if (_q.IsCompleted.HasValue)
            query = query.Where(p => p.IsCompleted == _q.IsCompleted.Value);

        if (_q.DiagnosisId.HasValue && _q.DiagnosisId.Value > 0)
            query = query.Where(p => p.DiagnosisId == _q.DiagnosisId.Value);

        if (_q.PatientId.HasValue && _q.PatientId.Value > 0)
            query = query.Where(p =>
                p.Diagnosis != null &&
                p.Diagnosis.PatientId == _q.PatientId.Value);

        if (_q.TicketId.HasValue && _q.TicketId.Value > 0)
            query = query.Where(p => p.TicketId == _q.TicketId.Value);

        if (_q.PaymentReference.HasValue)
            query = query.Where(p => p.PaymentReference == _q.PaymentReference.Value);

        return query;
    }

    // ---------------- SEARCH ----------------
    private IQueryable<Payment> ApplySearch(IQueryable<Payment> query)
    {
        if (string.IsNullOrWhiteSpace(_q.SearchTerm))
            return query;

        var pattern = $"%{_q.SearchTerm!.Trim().ToLower()}%";

        return query.Where(p =>
            p.Diagnosis != null &&
            (
                EF.Functions.Like(p.Diagnosis.DiagnosisText.ToLower(), pattern) ||
                (p.Diagnosis.Patient != null &&
                 p.Diagnosis.Patient.Person != null &&
                 EF.Functions.Like(p.Diagnosis.Patient.Person.FullName.ToLower(), pattern))
            ));
    }

    // ---------------- SORTING ----------------
    private IQueryable<Payment> ApplySorting(IQueryable<Payment> query)
    {
        var col = _q.SortColumn?.Trim().ToLowerInvariant() ?? "createdatutc";
        var isDesc = string.Equals(_q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "totalamount" => isDesc
                ? query.OrderByDescending(p => p.TotalAmount)
                : query.OrderBy(p => p.TotalAmount),

            "paidamount" => isDesc
                ? query.OrderByDescending(p => p.PaidAmount)
                : query.OrderBy(p => p.PaidAmount),

            "patient" => isDesc
                ? query.OrderByDescending(p => p.Diagnosis!.Patient!.Person!.FullName)
                : query.OrderBy(p => p.Diagnosis!.Patient!.Person!.FullName),

            "completed" => isDesc
                ? query.OrderByDescending(p => p.IsCompleted)
                : query.OrderBy(p => p.IsCompleted),

            _ => isDesc
                ? query.OrderByDescending(p => p.CreatedAtUtc)
                : query.OrderBy(p => p.CreatedAtUtc),
        };
    }
}
