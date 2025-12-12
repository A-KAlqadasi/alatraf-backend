using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Appointments.Commands.RescheduleAppointment;

public sealed record class RescheduleAppointmentCommand(
    int AppointmentId,
    DateOnly NewAttendDate) : IRequest<Result<Updated>>;