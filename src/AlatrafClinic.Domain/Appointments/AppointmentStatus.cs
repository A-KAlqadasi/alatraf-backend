namespace AlatrafClinic.Domain.Appointments;

public enum AppointmentStatus : byte
{
    Scheduled = 0,
    Cancelled,
    Today,
    Absent,
    Attended
}