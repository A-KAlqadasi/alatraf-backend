using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlatrafClinic.Application.Features.Appointments.Dtos
{
    public class AppointmentDto
    { 
     public int Id { get; set; }
    public int TicketId { get; set; }
    public string PatientType { get; set; } = string.Empty;
    public DateTime AttendDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }

    // Optional: to show if the appointment can be edited or rescheduled
    public bool IsEditable { get; set; }

    // Optional: for UI / reminder logic
    public bool IsAppointmentTomorrow { get; set; } 
    }
}