using AlatrafClinic.Application.Features.Tickets.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Tickets.Queries.GetTicketForScheduleAppointment;

public sealed record GetTicketForScheduleAppointmentQuery(
    int TicketId 
) : IRequest<Result<TicketForServiceDto>>;
