using AlatrafClinic.Application.Features.Appointments.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Appointments.Queries.GetAppointmentById;

public sealed record GetAppointmentByIdQuery(int AppointmentId) : IRequest<Result<AppointmentDto>>;