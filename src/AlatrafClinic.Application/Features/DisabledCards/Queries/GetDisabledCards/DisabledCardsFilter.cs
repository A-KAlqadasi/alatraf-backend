using AlatrafClinic.Application.Common.Specifications;
using AlatrafClinic.Domain.DisabledCards;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.DisabledCards.Queries.GetDisabledCards;

public sealed class DisabledCardsFilter : FilterSpecification<DisabledCard>
{
    private readonly GetDisabledCardsQuery _q;

    public DisabledCardsFilter(GetDisabledCardsQuery q)
        : base(q.Page, q.PageSize)
    {
        _q = q;
    }

    public override IQueryable<DisabledCard> Apply(IQueryable<DisabledCard> query)
    {
        query = query
            .Include(dc => dc.Patient)
                .ThenInclude(p => p.Person);

        query = ApplyFilters(query);
        query = ApplySearch(query);
        query = ApplySorting(query);

        return query;
    }

    // ---------------- FILTERS ----------------
    private IQueryable<DisabledCard> ApplyFilters(IQueryable<DisabledCard> query)
    {
        // filter by "expired" using ExpirationDate vs today
        if (_q.IsExpired.HasValue)
        {
            var today = DateTime.Today;
            if (_q.IsExpired.Value)
            {
                query = query.Where(dc => dc.ExpirationDate < today);
            }
            else
            {
                query = query.Where(dc => dc.ExpirationDate >= today);
            }
        }

        if (_q.PatientId.HasValue && _q.PatientId.Value > 0)
        {
            var patientId = _q.PatientId.Value;
            query = query.Where(dc => dc.PatientId == patientId);
        }

        if (_q.ExpirationFrom.HasValue)
        {
            var from = _q.ExpirationFrom.Value.Date;
            query = query.Where(dc => dc.ExpirationDate >= from);
        }

        if (_q.ExpirationTo.HasValue)
        {
            var to = _q.ExpirationTo.Value.Date;
            query = query.Where(dc => dc.ExpirationDate <= to);
        }

        return query;
    }

    // ---------------- SEARCH ----------------
    private IQueryable<DisabledCard> ApplySearch(IQueryable<DisabledCard> query)
    {
        if (string.IsNullOrWhiteSpace(_q.SearchTerm))
            return query;

        var pattern = $"%{_q.SearchTerm!.Trim().ToLower()}%";

        return query.Where(dc =>
            EF.Functions.Like(dc.CardNumber.ToLower(), pattern) ||
            (dc.Patient != null &&
             dc.Patient.Person != null &&
             EF.Functions.Like(dc.Patient.Person.FullName.ToLower(), pattern)));
    }

    // ---------------- SORTING ----------------
    private IQueryable<DisabledCard> ApplySorting(IQueryable<DisabledCard> query)
    {
        var col = _q.SortColumn?.Trim().ToLowerInvariant() ?? "expirationdate";
        var isDesc = string.Equals(_q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "cardnumber" => isDesc
                ? query.OrderByDescending(dc => dc.CardNumber)
                : query.OrderBy(dc => dc.CardNumber),

            "patient" => isDesc
                ? query.OrderByDescending(dc => dc.Patient!.Person!.FullName)
                : query.OrderBy(dc => dc.Patient!.Person!.FullName),

            "expirationdate" => isDesc
                ? query.OrderByDescending(dc => dc.ExpirationDate)
                : query.OrderBy(dc => dc.ExpirationDate),

            _ => isDesc
                ? query.OrderByDescending(dc => dc.ExpirationDate)
                : query.OrderBy(dc => dc.ExpirationDate),
        };
    }
}
