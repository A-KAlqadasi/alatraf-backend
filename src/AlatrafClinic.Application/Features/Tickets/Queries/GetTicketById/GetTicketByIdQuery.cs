using AlatrafClinic.Application.Features.Tickets.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Tickets.Queries.GetTicketById;

public sealed record GetTicketByIdQuery(int ticketId) : IRequest<Result<TicketDto>>;