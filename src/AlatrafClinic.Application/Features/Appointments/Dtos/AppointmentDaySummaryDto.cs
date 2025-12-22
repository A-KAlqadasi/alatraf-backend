namespace AlatrafClinic.Application.Features.Appointments.Dtos;

public sealed record AppointmentDaySummaryDto(
    DateOnly Date,
    int AppointmentsCount
);
