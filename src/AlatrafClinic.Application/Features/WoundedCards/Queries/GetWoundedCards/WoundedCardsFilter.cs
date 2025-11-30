using AlatrafClinic.Application.Common.Specifications;
using AlatrafClinic.Domain.WoundedCards;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.WoundedCards.Queries.GetWoundedCards;

public sealed class WoundedCardsFilter : FilterSpecification<WoundedCard>
{
    private readonly GetWoundedCardsQuery _q;

    public WoundedCardsFilter(GetWoundedCardsQuery q)
        : base(q.Page, q.PageSize)
    {
        _q = q;
    }

    public override IQueryable<WoundedCard> Apply(IQueryable<WoundedCard> query)
    {
        query = query
            .Include(wc => wc.Patient)
                .ThenInclude(p => p.Person);

        query = ApplyFilters(query);
        query = ApplySearch(query);
        query = ApplySorting(query);

        return query;
    }

    // ---------------- FILTERS ----------------
    private IQueryable<WoundedCard> ApplyFilters(IQueryable<WoundedCard> query)
    {
        var today = DateTime.Today;

        if (_q.IsExpired.HasValue)
        {
            if (_q.IsExpired.Value)
            {
                query = query.Where(wc => wc.Expiration < today);
            }
            else
            {
                query = query.Where(wc => wc.Expiration >= today);
            }
        }

        if (_q.PatientId.HasValue && _q.PatientId.Value > 0)
        {
            var patientId = _q.PatientId.Value;
            query = query.Where(wc => wc.PatientId == patientId);
        }

        if (_q.ExpirationFrom.HasValue)
        {
            var from = _q.ExpirationFrom.Value.Date;
            query = query.Where(wc => wc.Expiration >= from);
        }

        if (_q.ExpirationTo.HasValue)
        {
            var to = _q.ExpirationTo.Value.Date;
            query = query.Where(wc => wc.Expiration <= to);
        }

        return query;
    }

    // ---------------- SEARCH ----------------
    private IQueryable<WoundedCard> ApplySearch(IQueryable<WoundedCard> query)
    {
        if (string.IsNullOrWhiteSpace(_q.SearchTerm))
            return query;

        var pattern = $"%{_q.SearchTerm!.Trim().ToLower()}%";

        return query.Where(wc =>
            EF.Functions.Like(wc.CardNumber.ToLower(), pattern) ||
            (wc.Patient != null &&
             wc.Patient.Person != null &&
             EF.Functions.Like(wc.Patient.Person.FullName.ToLower(), pattern)));
    }

    // ---------------- SORTING ----------------
    private IQueryable<WoundedCard> ApplySorting(IQueryable<WoundedCard> query)
    {
        var col = _q.SortColumn?.Trim().ToLowerInvariant() ?? "expiration";
        var isDesc = string.Equals(_q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "cardnumber" => isDesc
                ? query.OrderByDescending(wc => wc.CardNumber)
                : query.OrderBy(wc => wc.CardNumber),

            "patient" => isDesc
                ? query.OrderByDescending(wc => wc.Patient!.Person!.FullName)
                : query.OrderBy(wc => wc.Patient!.Person!.FullName),

            "expiration" => isDesc
                ? query.OrderByDescending(wc => wc.Expiration)
                : query.OrderBy(wc => wc.Expiration),

            _ => isDesc
                ? query.OrderByDescending(wc => wc.Expiration)
                : query.OrderBy(wc => wc.Expiration),
        };
    }
}

