using AlatrafClinic.Domain.Services.Appointments.Holidays;

namespace AlatrafClinic.Application.Features.Appointments.Shared;

public static class AppointmentSchedulingCalculator
{
    public static IReadOnlyCollection<DayOfWeek> ParseAllowedDaysOrDefault(string? allowedDaysString)
    {
        var fallback = new[]
        {
            DayOfWeek.Saturday,
            DayOfWeek.Sunday,
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday
        };

        if (string.IsNullOrWhiteSpace(allowedDaysString))
            return fallback;

        var parts = allowedDaysString.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var days = new List<DayOfWeek>(parts.Length);
        foreach (var p in parts)
        {
            if (Enum.TryParse<DayOfWeek>(p, ignoreCase: true, out var d))
                days.Add(d);
        }

        return days.Count > 0 ? days : fallback;
    }

    public static bool IsValidAppointmentDay(DateOnly date, IReadOnlyCollection<DayOfWeek> allowedDays, IReadOnlyCollection<Holiday> holidays)
    {
        if (date.DayOfWeek == DayOfWeek.Friday)
            return false;

        if (!allowedDays.Contains(date.DayOfWeek))
            return false;

        if (holidays.Any(h => h.Matches(date)))
            return false;

        return true;
    }

    public static DateOnly GetNextValidDateInclusive(DateOnly start, IReadOnlyCollection<DayOfWeek> allowedDays, IReadOnlyCollection<Holiday> holidays)
    {
        var validDate = start;
        while (!IsValidAppointmentDay(validDate, allowedDays, holidays))
            validDate = validDate.AddDays(1);

        return validDate;
    }
    public static DateOnly GetNextValidDateExclusive(DateOnly after, IReadOnlyCollection<DayOfWeek> allowedDays, IReadOnlyCollection<Holiday> holidays)
    {
        return GetNextValidDateInclusive(after.AddDays(1), allowedDays, holidays);
    }

    public static async Task<DateOnly> FindNextValidDateWithCapacityAsync(
        DateOnly startInclusive,
        IReadOnlyCollection<DayOfWeek> allowedDays,
        IReadOnlyCollection<Holiday> holidays,
        int dailyCapacity,
        Func<DateOnly, CancellationToken, Task<int>> getCountForDateAsync,
        CancellationToken ct)
    {
        if (dailyCapacity <= 0)
            dailyCapacity = 1;

        var date = startInclusive;

        while (true)
        {
            // 1) first make it a “valid” date by calendar rules
            date = GetNextValidDateInclusive(date, allowedDays, holidays);

            // 2) then enforce capacity
            var count = await getCountForDateAsync(date, ct);
            if (count < dailyCapacity)
                return date;

            // Day is full -> move forward and try again
            date = date.AddDays(1);
        }
    }
}