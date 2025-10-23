namespace AlatrafClinic.Domain.Services.Appointments.Enums;

public enum AppointmentState : byte
{
    Scheduled = 0,
    Cancelled,
    Today,
    Absent,
    Attended
}