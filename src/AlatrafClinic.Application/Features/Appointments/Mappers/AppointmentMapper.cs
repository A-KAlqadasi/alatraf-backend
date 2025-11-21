using AlatrafClinic.Application.Features.Appointments.Dtos;
using AlatrafClinic.Application.Features.Tickets.Mappers;
using AlatrafClinic.Domain.Services.Appointments;


namespace AlatrafClinic.Application.Features.Appointments.Mappers;

public static class AppointmentMapper
{
    public static AppointmentDto ToDto(this Appointment appointment)
    {
        ArgumentNullException.ThrowIfNull(appointment);
        return new AppointmentDto
        {
            Id = appointment.Id,
            TicketId = appointment.TicketId,
            Ticket = appointment.Ticket?.ToDto(),
            PatientType = appointment.PatientType,
            AttendDate = appointment.AttendDate,
            Status = appointment.Status,
            Notes = appointment.Notes,
            CreatedAt = appointment.CreatedAtUtc.DateTime.Date,
            IsEditable = appointment.IsEditable,
            IsAppointmentTomorrow = appointment.IsAppointmentTomorrow()
        };
    }
    public static List<AppointmentDto> ToDtos(this IEnumerable<Appointment> appointments)
    {
        return appointments.Select(a => a.ToDto()).ToList();
    }
}