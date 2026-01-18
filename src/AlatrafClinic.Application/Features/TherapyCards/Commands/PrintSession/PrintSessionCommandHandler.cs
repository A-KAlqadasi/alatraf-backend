using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Printing;
using AlatrafClinic.Application.Common.Printing.Dtos;
using AlatrafClinic.Application.Common.Printing.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Sessions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.PrintSession;

public class PrintSessionCommandHandler
    : IRequestHandler<PrintSessionCommand, Result<PdfDto>>
{
    private readonly IPdfGenerator<Session> _sessionPdfGenerator;
    private readonly IAppDbContext _context;
    private readonly ILogger<PrintSessionCommandHandler> _logger;
    public PrintSessionCommandHandler(
        IPdfGenerator<Session> sessionPdfGenerator,
        IAppDbContext context, ILogger<PrintSessionCommandHandler> logger)
    {
        _sessionPdfGenerator = sessionPdfGenerator;
        _context = context;
        _logger = logger;
    }

    public async Task<Result<PdfDto>> Handle(
        PrintSessionCommand command,
        CancellationToken ct)
    {
       var session = await _context.Sessions
            .Include(s => s.TherapyCard)
                .ThenInclude(s=> s.Diagnosis)
                .ThenInclude(tc => tc.Patient)
                    .ThenInclude(p => p.Person)
            .Include(s => s.SessionPrograms)
                .ThenInclude(sp => sp.DiagnosisProgram)
                    .ThenInclude(dp => dp.MedicalProgram)
            .Include(s => s.SessionPrograms)
                .ThenInclude(sp => sp.DoctorSectionRoom)
                    .ThenInclude(dsr => dsr!.Section)
            .Include(s => s.SessionPrograms)
                .ThenInclude(sp => sp.DoctorSectionRoom)
                    .ThenInclude(dsr => dsr!.Room)
            .Include(s => s.SessionPrograms)
                .ThenInclude(sp => sp.DoctorSectionRoom)
                    .ThenInclude(dsr => dsr!.Doctor)
                        .ThenInclude(d => d.Person)
            .AsNoTracking()
            .FirstOrDefaultAsync(s=> s.Id == command.SessionId, ct);

        if (session is null)
        {
            return Error.NotFound(
                $"كرت الجلسة بالمعرف {command.SessionId} غير موجود.");
        }


        var printedDocument =
            await _context.PrintedDocuments.GetOrCreateAsync(
                nameof(DocumentTypes.Session),
                session.Id,
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
            var pdfBytes = _sessionPdfGenerator.Generate(session, printContext);

            return new PdfDto
            {
                Content = pdfBytes,
                FileName = $"Session_{session.Id}.pdf",
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate PDF for SessionId: {SessionId}", command.SessionId);
            return Error.Failure("An error occurred while generating the session PDF.");
        }
    }
}