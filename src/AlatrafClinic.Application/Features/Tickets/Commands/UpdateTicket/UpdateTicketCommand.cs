using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Tickets.Commands.UpdateTicket;

public record class UpdateTicketCommand(int TicketId, int ServiceId, int PatientId) : IRequest<Result<Updated>>;