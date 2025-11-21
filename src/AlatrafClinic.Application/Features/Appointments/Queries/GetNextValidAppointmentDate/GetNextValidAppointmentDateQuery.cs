using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Appointments.Queries.GetNextValidAppointmentDate;

public sealed record GetNextValidAppointmentDateQuery( DateTime RequestedDate = default
) : IRequest<Result<DateTime>>;
