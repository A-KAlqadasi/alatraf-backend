using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Enums;

namespace AlatrafClinic.Domain.Services.Appointments.Holidays;

public sealed class Holiday : AuditableEntity<int>
{
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public string? Name { get; private set; }
    public bool IsRecurring { get; private set; }
    public bool IsActive { get; private set; }
    public HolidayType Type { get; private set; }
    private Holiday() { }

    private Holiday(DateTime startDate, string? name, bool isRecurring, HolidayType type, bool isActive, DateTime? endDate = null)
    {
        StartDate = startDate.Date;
        Name = name;
        IsRecurring = isRecurring;
        Type = type;
        EndDate = endDate?.Date;
        IsActive = isActive;
    }


    public static Result<Holiday> CreateFixed(DateTime date, string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return HolidayErrors.HolidayNameIsRequired;

        if (date.Year != 1)
            return HolidayErrors.HolidayFixedDateYearMustBeOne;

        return new Holiday(date, name, isRecurring: true, HolidayType.Fixed, isActive: true);
    }




    public static Result<Holiday> CreateTemporary(DateTime startDate, string? name, DateTime? endDate = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return HolidayErrors.HolidayNameIsRequired;

        if (endDate.HasValue && endDate.Value.Date < startDate.Date)
            return HolidayErrors.HolidayEndDateBeforeStartDate;

        return new Holiday(startDate, name, isRecurring: false, HolidayType.Temporary, isActive: false, endDate: endDate);
    }

    public bool Matches(DateTime target)
    {
        if (!IsActive)
            return false;

        if (IsRecurring)
        {
            if (EndDate.HasValue)
            {
                var start = new DateTime(target.Year, StartDate.Month, StartDate.Day);
                var end = new DateTime(target.Year, EndDate.Value.Month, EndDate.Value.Day);
                return target.Date >= start.Date && target.Date <= end.Date;
            }

            return StartDate.Day == target.Day && StartDate.Month == target.Month;
        }
        else
        {
            if (EndDate.HasValue)
                return target.Date >= StartDate.Date && target.Date <= EndDate.Value.Date;

            return StartDate.Date == target.Date;
        }
    }



    public Result<Updated> UpdateHoliday(
        string name,
        DateTime startDate,
        DateTime? endDate,
        bool isRecurring,
        HolidayType type)
    {

        if (string.IsNullOrWhiteSpace(name))
            return HolidayErrors.HolidayNameIsRequired;

        if (!Enum.IsDefined(typeof(HolidayType), type))
            return HolidayErrors.InvalidHolidayType;


        if (type == HolidayType.Fixed && startDate.Year != 1)
            return HolidayErrors.HolidayFixedDateYearMustBeOne;

        if (endDate.HasValue && endDate.Value.Date < startDate.Date)
            return HolidayErrors.HolidayEndDateBeforeStartDate;


        // if (type == HolidayType.Fixed && !isRecurring)
        //     return HolidayErrors.FixedHolidayMustBeRecurring;

        Name = name;
        StartDate = startDate.Date;
        EndDate = endDate?.Date;
        IsRecurring = isRecurring;
        Type = type;

        return Result.Updated;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

}