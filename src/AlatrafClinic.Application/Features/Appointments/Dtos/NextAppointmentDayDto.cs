namespace AlatrafClinic.Application.Features.Appointments.Dtos;

public sealed record NextAppointmentDayDto(
    DateOnly Date,
    int AppointmentsCountOnThatDate
);