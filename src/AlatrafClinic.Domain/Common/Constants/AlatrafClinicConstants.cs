namespace AlatrafClinic.Domain.Common.Constants;

public static class AlatrafClinicConstants
{

    public const string SystemUser = "System";
    public const string AllowedDaysKey = "AllowedAppointmentDays";
    public const string AppointmentDailyCapacityKey = "AppointmentDailyCapacity";
    public const string WoundedReportMinTotalKey = "WoundedReportMinTotal";
    public const int DefaultAppointmentDailyCapacity = 10;

    public static readonly DateOnly TodayDate = DateOnly.FromDateTime(DateTime.Now);
}