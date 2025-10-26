using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Services.Appointments.Holidays;

public sealed class Holiday : AuditableEntity<int>
{
    public DateTime Date { get; private set; }
    public string? Name { get; private set; }
    public bool IsRecurring { get; private set; } // true = repeats every year

    private Holiday() { }

    private Holiday(DateTime date, string? name, bool isRecurring)
    {
        Date = date.Date;
        Name = name;
        IsRecurring = isRecurring;
    }

    public static Result<Holiday> CreateFixed(DateTime date, string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return HolidayErrors.HolidayNameIsRequired;
        }
        if (date.Year != 1)
        {
            return HolidayErrors.HolidayFixedDateYearMustBeOne;
        }
        
        return new Holiday(date, name, isRecurring: true);
    }
        

    public static Result<Holiday> CreateTemporary(DateTime date, string? name)
        => new Holiday(date, name, isRecurring: false);

    public bool Matches(DateTime target)
    {
        return IsRecurring
            ? Date.Day == target.Day && Date.Month == target.Month
            : Date.Date == target.Date;
    }
}