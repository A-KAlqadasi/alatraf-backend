
using AlatrafClinic.Application.Features.Tickets.Dtos;
using AlatrafClinic.Domain.Patients.Enums;
using AlatrafClinic.Domain.Services.Enums;

namespace AlatrafClinic.Application.Features.Appointments.Dtos;

public class AppointmentDto
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public TicketDto? Ticket { get; set; }
    public PatientType PatientType { get; set; } 
    public DateTime AttendDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public AppointmentStatus Status { get; set; } 
    public string? Notes { get; set; }
    public bool IsEditable { get; set; }
    public bool IsAppointmentTomorrow { get; set; }
}
