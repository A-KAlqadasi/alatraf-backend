using AlatrafClinic.Application.Common.Specifications;
using AlatrafClinic.Domain.Services.Appointments.Holidays;

namespace AlatrafClinic.Application.Features.Holidays.Queries.GetHolidays;

public sealed class HolidaysFilter : FilterSpecification<Holiday>
{
    private readonly GetHolidaysQuery _q;

    public HolidaysFilter(GetHolidaysQuery q)
        : base(q.Page, q.PageSize)
    {
        _q = q;
    }

    public override IQueryable<Holiday> Apply(IQueryable<Holiday> query)
    {
        query = ApplyFilters(query);
        query = ApplySorting(query);
        return query;
    }

    // ---------------- FILTERS ----------------
    private IQueryable<Holiday> ApplyFilters(IQueryable<Holiday> query)
    {
        if (_q.IsActive.HasValue)
            query = query.Where(h => h.IsActive == _q.IsActive.Value);

        if (_q.Type.HasValue)
            query = query.Where(h => h.Type == _q.Type.Value);

        if (_q.EndDate.HasValue)
        {
            var end = _q.EndDate.Value.Date;
            query = query.Where(h => h.EndDate.HasValue && h.EndDate.Value.Date == end);
        }

        if (_q.SpecificDate.HasValue)
        {
            var date = _q.SpecificDate.Value.Date;

            // Same logic you had in ApplyFilters, just moved here
            query = query.Where(h =>
                h.IsActive &&
                (
                    // Recurring range holiday (same month/day range)
                    (h.IsRecurring && h.EndDate.HasValue &&
                     new DateTime(date.Year, h.StartDate.Month, h.StartDate.Day) <= date &&
                     new DateTime(date.Year, h.EndDate.Value.Month, h.EndDate.Value.Day) >= date)
                    ||
                    // Recurring one-day holiday
                    (h.IsRecurring && !h.EndDate.HasValue &&
                     h.StartDate.Month == date.Month &&
                     h.StartDate.Day == date.Day)
                    ||
                    // Temporary range holiday
                    (!h.IsRecurring && h.EndDate.HasValue &&
                     h.StartDate.Date <= date && h.EndDate.Value.Date >= date)
                    ||
                    // Temporary one-day holiday
                    (!h.IsRecurring && !h.EndDate.HasValue &&
                     h.StartDate.Date == date)
                )
            );
        }

        return query;
    }

    // ---------------- SORTING ----------------
    private IQueryable<Holiday> ApplySorting(IQueryable<Holiday> query)
    {
        var col = _q.SortBy?.Trim().ToLowerInvariant() ?? "startdate";
        var desc = string.Equals(_q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "name" =>
                desc ? query.OrderByDescending(h => h.Name)
                     : query.OrderBy(h => h.Name),

            "type" =>
                desc ? query.OrderByDescending(h => h.Type)
                     : query.OrderBy(h => h.Type),

            "isactive" =>
                desc ? query.OrderByDescending(h => h.IsActive)
                     : query.OrderBy(h => h.IsActive),

            "enddate" =>
                desc ? query.OrderByDescending(h => h.EndDate)
                     : query.OrderBy(h => h.EndDate),

            _ => // default: startdate
                desc ? query.OrderByDescending(h => h.StartDate)
                     : query.OrderBy(h => h.StartDate),
        };
    }
}
