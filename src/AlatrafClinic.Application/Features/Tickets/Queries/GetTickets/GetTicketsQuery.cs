using AlatrafClinic.Application.Features.Tickets.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Tickets.Queries.GetTickets;

public record class GetTicketsQuery : IRequest<Result<List<TicketDto>>>
{
}