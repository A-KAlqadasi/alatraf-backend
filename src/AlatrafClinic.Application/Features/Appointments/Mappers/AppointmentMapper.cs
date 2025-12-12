using AlatrafClinic.Application.Features.Appointments.Dtos;
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
            PatientName = appointment.Ticket?.Patient?.Person.FullName ?? string.Empty,
            PatientType = appointment.PatientType,
            AttendDate = appointment.AttendDate,
            Status = appointment.Status,
            Notes = appointment.Notes,
            CreatedAt = DateOnly.FromDateTime(appointment.CreatedAtUtc.DateTime),
            IsAppointmentTomorrow = appointment.IsAppointmentTomorrow()
        };
    }
    public static List<AppointmentDto> ToDtos(this IEnumerable<Appointment> appointments)
    {
        return appointments.Select(a => a.ToDto()).ToList();
    }
}