namespace AlatrafClinic.Domain.Common.Constants;

public static class AlatrafClinicConstants
{

    public const string SystemUser = "System";
    public const string AllowedDaysKey = "AllowedAppointmentDays";
    public const string AllowedScheduledPatientsKey = "AllowedScheduledPatients";
    public const string WoundedReportMinTotalKey = "WoundedReportMinTotal";
    public static readonly DateOnly TodayDate = DateOnly.FromDateTime(DateTime.UtcNow);
}