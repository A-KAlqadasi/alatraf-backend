using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Enums;

using MediatR;

namespace AlatrafClinic.Application.Features.Tickets.Commands.UpdateTicket;

public record class UpdateTicketCommand(int TicketId, int ServiceId, int PatientId, TicketStatus? Status) : IRequest<Result<Updated>>;