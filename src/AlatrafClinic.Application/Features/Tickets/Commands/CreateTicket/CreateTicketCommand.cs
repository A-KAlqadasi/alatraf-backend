using AlatrafClinic.Application.Features.Tickets.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Tickets.Commands.CreateTicket;

public record class CreateTicketCommand(int ServiceId, int? PatientId) : IRequest<Result<TicketDto>>;