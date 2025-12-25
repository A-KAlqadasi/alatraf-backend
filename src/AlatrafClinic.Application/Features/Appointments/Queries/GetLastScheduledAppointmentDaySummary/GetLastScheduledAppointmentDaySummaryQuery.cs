using AlatrafClinic.Application.Features.Appointments.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Appointments.Queries.GetLastScheduledAppointmentDaySummary;

public sealed record GetLastScheduledAppointmentDaySummaryQuery()
    : IRequest<Result<AppointmentDaySummaryDto>>;
