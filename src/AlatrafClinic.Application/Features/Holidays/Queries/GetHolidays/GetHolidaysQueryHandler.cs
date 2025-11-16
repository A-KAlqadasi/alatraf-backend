

using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Holidays.Dtos;
using AlatrafClinic.Application.Features.Holidays.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Appointments.Holidays;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Holidays.Queries.GetHolidays;

public interface IGetHolidaysQueryHandler
{
    Task<Result<List<HolidayDto>>> Handle(GetHolidaysQuery query, CancellationToken ct);
}

public class GetHolidaysQueryHandler
    : IRequestHandler<GetHolidaysQuery, Result<List<HolidayDto>>>, IGetHolidaysQueryHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public GetHolidaysQueryHandler(IUnitOfWork uow)
    {
        _unitOfWork = uow;
    }

    public async Task<Result<List<HolidayDto>>> Handle(GetHolidaysQuery query, CancellationToken ct)
    {
        var holidayQuery =await _unitOfWork.Holidays.GetHolidaysQueryAsync(ct);


    
        holidayQuery = ApplyFilters(holidayQuery, query);


        holidayQuery = ApplySorting(holidayQuery, query.SortBy, query.SortDirection);

        var holdies = await holidayQuery.ToListAsync(ct);

        return holdies.ToDtos();
    }

    private static IQueryable<Holiday> ApplyFilters(IQueryable<Holiday> query, GetHolidaysQuery q)
    {
        if (q.IsActive.HasValue)
            query = query.Where(h => h.IsActive == q.IsActive.Value);

        if (q.Type.HasValue)
            query = query.Where(h => h.Type == q.Type.Value);

        if (q.EndDate.HasValue)
        {
            var end = q.EndDate.Value.Date;
            query = query.Where(h => h.EndDate.HasValue && h.EndDate.Value.Date == end);
        }

        if (q.SpecificDate.HasValue)
        {
            var date = q.SpecificDate.Value.Date;

            // Equivalent to Matches(date), but EF-compatible
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

    private static IQueryable<Holiday> ApplySorting(IQueryable<Holiday> query, string? sortBy, string? sortDir)
    {
        var col = sortBy?.Trim().ToLowerInvariant() ?? "startdate";
        var desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);

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