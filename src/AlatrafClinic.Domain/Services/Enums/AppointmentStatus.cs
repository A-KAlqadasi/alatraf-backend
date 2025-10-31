namespace AlatrafClinic.Domain.Services.Enums;

public enum AppointmentStatus : byte
{
    Scheduled = 0,
    Cancelled,
    Today,
    Absent,
    Attended
}