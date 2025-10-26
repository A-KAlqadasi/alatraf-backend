namespace AlatrafClinic.Domain.Services.Appointments.Holidays;

public sealed class HolidayCalendar
{
    private readonly List<Holiday> _holidays = new();

    public HolidayCalendar(IEnumerable<Holiday> holidays)
    {
        _holidays = holidays.ToList();
    }

    public bool IsHoliday(DateTime date)
    {
        if (date.DayOfWeek == DayOfWeek.Friday)
            return true;
            
        return _holidays.Any(h => h.Matches(date));
    }
}