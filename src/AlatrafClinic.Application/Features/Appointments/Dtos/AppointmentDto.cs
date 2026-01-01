
using AlatrafClinic.Application.Features.Tickets.Dtos;
using AlatrafClinic.Domain.Patients.Enums;
using AlatrafClinic.Domain.Services.Enums;

namespace AlatrafClinic.Application.Features.Appointments.Dtos;

public class AppointmentDto
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string PatientType { get; set; } = string.Empty;
    public DateOnly AttendDate { get; set; }
    public DateOnly CreatedAt { get; set; }
    public string DayOfWeek { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public bool IsAppointmentTomorrow { get; set; }
}
