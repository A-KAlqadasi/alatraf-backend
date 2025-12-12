namespace AlatrafClinic.Domain.Common.Constants;

public static class AlatrafClinicConstants
{

    public const string SystemUser = "System";
    public const string AllowedDaysKey = "AllowedAppointmentDays";
    public static readonly DateOnly TodayDate = DateOnly.FromDateTime(DateTime.UtcNow);
}