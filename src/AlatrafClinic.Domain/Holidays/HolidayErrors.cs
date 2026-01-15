using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Holidays;

public static class HolidayErrors
{
    public static readonly Error HolidayNameIsRequired = Error.Validation(
        code: "Holiday.NameRequired",
        description: "اسم العطلة مطلوب");
    public static readonly Error HolidayFixedDateYearMustBeOne = Error.Validation(
        code: "Holiday.FixedDateYearMustBeOne",
        description: "يجب أن يكون سنة تاريخ العطلة الثابتة 1");

    public static readonly Error HolidayEndDateBeforeStartDate = Error.Failure("لا يمكن أن يكون تاريخ نهاية العطلة قبل تاريخ البدء");
    public static readonly Error InvalidHolidayType =
       Error.Validation("InvalidHolidayType", "نوع العطلة غير صالح");
}