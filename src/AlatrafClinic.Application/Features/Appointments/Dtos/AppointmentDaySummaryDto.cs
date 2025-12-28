namespace AlatrafClinic.Application.Features.Appointments.Dtos;

public sealed record AppointmentDaySummaryDto(
    DateOnly Date,
    string DayOfWeek,
    int AppointmentsCount
);
