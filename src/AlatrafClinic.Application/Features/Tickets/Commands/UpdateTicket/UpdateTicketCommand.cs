using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Tickets;

using MediatR;

namespace AlatrafClinic.Application.Features.Tickets.Commands.UpdateTicket;

public record class UpdateTicketCommand(int TicketId, int ServiceId, int PatientId, TicketStatus? Status) : IRequest<Result<Updated>>;