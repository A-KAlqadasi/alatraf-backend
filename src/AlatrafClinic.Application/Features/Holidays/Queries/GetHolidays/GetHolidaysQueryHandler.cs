using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Holidays.Dtos;
using AlatrafClinic.Application.Features.Holidays.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Appointments.Holidays;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Holidays.Queries.GetHolidays;

public sealed class GetHolidaysQueryHandler
    : IRequestHandler<GetHolidaysQuery, Result<PaginatedList<HolidayDto>>>
{
    private readonly IAppDbContext _context;

    public GetHolidaysQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<HolidayDto>>> Handle(
        GetHolidaysQuery query,
        CancellationToken ct)
    {
        // Base query
        IQueryable<Holiday> holidaysQuery = _context.Holidays
            .AsNoTracking();

        // Apply filters & sorting
        holidaysQuery = ApplyFilters(holidaysQuery, query);
        holidaysQuery = ApplySorting(holidaysQuery, query);

        // Total count AFTER filters, BEFORE paging
        var totalCount = await holidaysQuery.CountAsync(ct);

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 10 : query.PageSize;
        var skip = (page - 1) * pageSize;

        // Page data
        var holidays = await holidaysQuery
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        var items = holidays.ToDtos();

        return new PaginatedList<HolidayDto>
        {
            Items      = items,
            PageNumber = page,
            PageSize   = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    // ---------------- FILTERS ----------------
    private static IQueryable<Holiday> ApplyFilters(
        IQueryable<Holiday> query,
        GetHolidaysQuery q)
    {
        if (q.IsActive.HasValue)
            query = query.Where(h => h.IsActive == q.IsActive.Value);

        if (q.Type.HasValue)
            query = query.Where(h => h.Type == q.Type.Value);

        if (q.EndDate.HasValue)
        {
            var end = q.EndDate.Value.Date;
            query = query.Where(h =>
                h.EndDate.HasValue && h.EndDate.Value.Date == end);
        }

        if (q.SpecificDate.HasValue)
        {
            var date = q.SpecificDate.Value.Date;

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
    private static IQueryable<Holiday> ApplySorting(
        IQueryable<Holiday> query,
        GetHolidaysQuery q)
    {
        var col  = q.SortBy?.Trim().ToLowerInvariant() ?? "startdate";
        var desc = string.Equals(q.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

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
