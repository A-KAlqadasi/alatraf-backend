using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Holidays;

public sealed class Holiday : AuditableEntity<int>
{
    public DateOnly StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public string? Name { get; private set; }
    public bool IsRecurring { get; private set; }
    public bool IsActive { get; private set; }
    public HolidayType Type { get; private set; }
    private Holiday() { }

    private Holiday(DateOnly startDate, string? name, bool isRecurring, HolidayType type, bool isActive, DateOnly? endDate = null)
    {
        StartDate = startDate;
        Name = name;
        IsRecurring = isRecurring;
        Type = type;
        EndDate = endDate;
        IsActive = isActive;
    }


    public static Result<Holiday> CreateFixed(DateOnly date, string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return HolidayErrors.HolidayNameIsRequired;

        if (date.Year != 1)
            return HolidayErrors.HolidayFixedDateYearMustBeOne;

        return new Holiday(date, name, isRecurring: true, HolidayType.Fixed, isActive: true);
    }




    public static Result<Holiday> CreateTemporary(DateOnly startDate, string? name, DateOnly? endDate = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return HolidayErrors.HolidayNameIsRequired;

        if (endDate.HasValue && endDate.Value < startDate)
            return HolidayErrors.HolidayEndDateBeforeStartDate;

        return new Holiday(startDate, name, isRecurring: false, HolidayType.Temporary, isActive: false, endDate: endDate);
    }

    public bool Matches(DateOnly target)
    {
        if (!IsActive)
            return false;

        if (IsRecurring)
        {
            if (EndDate.HasValue)
            {
                var start = new DateOnly(target.Year, StartDate.Month, StartDate.Day);
                var end = new DateOnly(target.Year, EndDate.Value.Month, EndDate.Value.Day);
                return target >= start && target <=end;
            }

            return StartDate.Day == target.Day && StartDate.Month == target.Month;
        }
        else
        {
            if (EndDate.HasValue)
                return target >= StartDate && target <= EndDate.Value;

            return StartDate == target;
        }
    }



    public Result<Updated> UpdateHoliday(
        string name,
        DateOnly startDate,
        DateOnly? endDate,
        bool isRecurring,
        HolidayType type)
    {

        if (string.IsNullOrWhiteSpace(name))
            return HolidayErrors.HolidayNameIsRequired;

        if (!Enum.IsDefined(typeof(HolidayType), type))
            return HolidayErrors.InvalidHolidayType;


        if (type == HolidayType.Fixed && startDate.Year != 1)
            return HolidayErrors.HolidayFixedDateYearMustBeOne;

        if (endDate.HasValue && endDate.Value < startDate)
            return HolidayErrors.HolidayEndDateBeforeStartDate;


        // if (type == HolidayType.Fixed && !isRecurring)
        //     return HolidayErrors.FixedHolidayMustBeRecurring;

        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        IsRecurring = isRecurring;
        Type = type;

        return Result.Updated;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

}