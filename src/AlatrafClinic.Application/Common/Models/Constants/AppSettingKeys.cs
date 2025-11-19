namespace AlatrafClinic.Application.Common.Models.Constants;

public static class AppSettingKeys
{
    public static class General
    {
        public const string WorkingDays = "Clinic.WorkingDays";
        public const string EnableDiscount = "General.EnableDiscount";
        public const string OpeningTime = "Clinic.OpeningTime";
        public const string ClosingTime = "Clinic.ClosingTime";

        public const string WorkOnWeekends = "Clinic.WorkOnWeekends";

        public const string ReportWorkingHour = "Reports.DailyHour";

    }
    public static class Appointment
    {
        public const string AllowedDaysForAppointments = "Appointment.AllowedDays";
        public const string MaxPerDay = "Appointment.MaxPerDay";
    }
    public static class Security
    {
        public const string PasswordExpirationDays = "Security.PasswordExpirationDays";
    }
}
