using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Holidays;

public static class HolidayErrors
{
    public static readonly Error HolidayNameIsRequired = Error.Validation(
        code: "Holiday.NameRequired",
        description: "Holiday name is required.");
    public static readonly Error HolidayFixedDateYearMustBeOne = Error.Validation(
        code: "Holiday.FixedDateYearMustBeOne",
        description: "Fixed holiday date must have year set to 1.");

    public static readonly Error HolidayEndDateBeforeStartDate = Error.Failure("Holiday end date cannot be before the start date.");
    public static readonly Error InvalidHolidayType =
       Error.Validation("InvalidHolidayType", "The holiday type is invalid.");
}