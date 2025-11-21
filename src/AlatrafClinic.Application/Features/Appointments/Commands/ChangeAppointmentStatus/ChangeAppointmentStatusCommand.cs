using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Enums;

using MediatR;

namespace AlatrafClinic.Application.Features.Appointments.Commands.ChangeAppointmentStatus;

public sealed record class ChangeAppointmentStatusCommand(
    int AppointmentId,
    AppointmentStatus NewStatus) : IRequest<Result<Updated>>;