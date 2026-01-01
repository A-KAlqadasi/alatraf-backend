namespace AlatrafClinic.Application.Features.Appointments.Dtos;

public sealed record NextAppointmentDayDto(
    DateOnly Date,
    string DayOfWeek,
    int AppointmentsCountOnThatDate
);