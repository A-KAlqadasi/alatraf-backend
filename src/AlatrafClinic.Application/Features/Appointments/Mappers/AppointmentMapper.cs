using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Appointments.Dtos;
using AlatrafClinic.Application.Features.Patients.Mappers;
using AlatrafClinic.Domain.Services.Appointments;
using AlatrafClinic.Domain.Services.Enums;


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
            PatientType = appointment.PatientType.ToArabicPatientType(),
            AttendDate = appointment.AttendDate,
            DayOfWeek = UtilityService.GetDayNameArabic(appointment.AttendDate),
            Status = appointment.Status.ToArabicAppointmentStatus(),
            Notes = appointment.Notes,
            CreatedAt = DateOnly.FromDateTime(appointment.CreatedAtUtc.DateTime),
            IsAppointmentTomorrow = appointment.IsAppointmentTomorrow()
        };
    }
    public static List<AppointmentDto> ToDtos(this IEnumerable<Appointment> appointments)
    {
        return appointments.Select(a => a.ToDto()).ToList();
    }
    public static string ToArabicAppointmentStatus(this AppointmentStatus status)
    {
        return status switch
        {
            AppointmentStatus.Scheduled => "في الانتظار",
            AppointmentStatus.Attended => "مكتمل",
            AppointmentStatus.Absent => "غائب",
            AppointmentStatus.Today => "اليوم",
            AppointmentStatus.Cancelled => "ملغي",
            _ => "غير معروف"
        };
    }
}