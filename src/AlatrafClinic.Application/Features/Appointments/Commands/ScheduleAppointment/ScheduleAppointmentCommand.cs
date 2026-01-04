using AlatrafClinic.Application.Features.Appointments.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Appointments.Commands.ScheduleAppointment;

public sealed record class ScheduleAppointmentCommand(int TicketId, string? Notes = null) : IRequest<Result<AppointmentDto>>;