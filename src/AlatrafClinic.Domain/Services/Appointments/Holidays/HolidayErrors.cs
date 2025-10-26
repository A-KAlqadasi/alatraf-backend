using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Services.Appointments.Holidays;

public static class HolidayErrors
{
    public static readonly Error HolidayNameIsRequired = Error.Validation(
        code: "Holiday.NameRequired",
        description: "Holiday name is required.");
    public static readonly Error HolidayFixedDateYearMustBeOne = Error.Validation(
        code: "Holiday.FixedDateYearMustBeOne",
        description: "Fixed holiday date must have year set to 1.");
}