using AlatrafClinic.Application.Features.Appointments.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Appointments.Queries.GetAppointmentCountsInDate;

public sealed record GetAppointmentCountsInDateQuery(DateOnly Date) : IRequest<Result<AppointmentCountsDto>>;