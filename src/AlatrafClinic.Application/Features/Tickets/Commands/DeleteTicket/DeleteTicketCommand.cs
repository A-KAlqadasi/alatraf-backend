using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Tickets.Commands.DeleteTicket;

public record class DeleteTicketCommand(int TicketId) : IRequest<Result<Deleted>>;