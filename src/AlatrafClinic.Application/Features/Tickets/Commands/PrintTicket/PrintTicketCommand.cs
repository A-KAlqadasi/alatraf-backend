using AlatrafClinic.Application.Common.Printing.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Tickets.Commands.PrintTicket;

public record PrintTicketCommand(int TicketId) : IRequest<Result<PdfDto>>;
