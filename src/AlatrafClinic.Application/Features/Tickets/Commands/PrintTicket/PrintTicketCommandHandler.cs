using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Printing;
using AlatrafClinic.Application.Common.Printing.Dtos;
using AlatrafClinic.Application.Common.Printing.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Tickets;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Tickets.Commands.PrintTicket;

public class PrintTicketCommandHandler
    : IRequestHandler<PrintTicketCommand, Result<PdfDto>>
{
    private readonly IPdfGenerator<Ticket> _ticketPdfGenerator;
    private readonly IAppDbContext _context;
    private readonly ILogger<PrintTicketCommandHandler> _logger;
    public PrintTicketCommandHandler(
        IPdfGenerator<Ticket> ticketPdfGenerator,
        IAppDbContext context, ILogger<PrintTicketCommandHandler> logger)
    {
        _ticketPdfGenerator = ticketPdfGenerator;
        _context = context;
        _logger = logger;
    }

    public async Task<Result<PdfDto>> Handle(
        PrintTicketCommand request,
        CancellationToken ct)
    {
        var ticket = await _context.Tickets
        .Include(t => t.Patient!)
            .ThenInclude(t=> t.Person)
        .Include(t => t.Service)
        .FirstOrDefaultAsync(
            t => t.Id == request.TicketId,
            ct);

        if (ticket is null)
        {
            return Error.NotFound(
                $"التذكرة بالمعرف {request.TicketId} غير موجودة.");
        }

        if (ticket.IsPrintable)
        {
            return Error.Conflict(
                $"التذكرة بالمعرف {request.TicketId} غير قابلة للطباعة.");
        }

        var printedDocument =
            await _context.PrintedDocuments.GetOrCreateAsync(
                nameof(DocumentTypes.Ticket),
                ticket.Id,
                ct);

        var printNumber = printedDocument.RegisterPrint();

        await _context.SaveChangesAsync(ct);

        var printContext = new PrintContext
        {
            PrintNumber = printNumber,
            PrintedAt = DateTime.Now
        };

        try
        {
            var pdfBytes = _ticketPdfGenerator.Generate(ticket, printContext);

            return new PdfDto
            {
                Content = pdfBytes,
                FileName = $"Ticket_{ticket.Id}.pdf",
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate PDF for TicketId: {TicketId}", request.TicketId);
            return Error.Failure("An error occurred while generating the ticket PDF.");
        }
       
    }
}