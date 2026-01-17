using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Printing;
using AlatrafClinic.Application.Common.Printing.Dtos;
using AlatrafClinic.Application.Common.Printing.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.PrintTherapyCard;

public class PrintTherapyCardCommandHandler
    : IRequestHandler<PrintTherapyCardCommand, Result<PdfDto>>
{
    private readonly IPdfGenerator<TherapyCard> _therapyCardPdfGenerator;
    private readonly IAppDbContext _context;
    private readonly ILogger<PrintTherapyCardCommandHandler> _logger;
    public PrintTherapyCardCommandHandler(
        IPdfGenerator<TherapyCard> therapyCardPdfGenerator,
        IAppDbContext context, ILogger<PrintTherapyCardCommandHandler> logger)
    {
        _therapyCardPdfGenerator = therapyCardPdfGenerator;
        _context = context;
        _logger = logger;
    }

    public async Task<Result<PdfDto>> Handle(
        PrintTherapyCardCommand command,
        CancellationToken ct)
    {
        var therapyCard = await _context.TherapyCards
        .AsNoTracking()
            .Include(tc => tc.Diagnosis)
                .ThenInclude(d => d.Patient)!.ThenInclude(p=> p.Person)
            .Include(tc => tc.Diagnosis)
                .ThenInclude(ir=> ir.InjuryReasons)
            .Include(tc => tc.Diagnosis)
                .ThenInclude(injs=> injs.InjurySides)
            .Include(tc => tc.Diagnosis)
                .ThenInclude(it=> it.InjuryTypes)
            .Include(tc => tc.DiagnosisPrograms)
                .ThenInclude(d=> d.MedicalProgram!).ThenInclude(d=> d.Section)
            .Include(tx=> tx.Diagnosis)
                .ThenInclude(tx=> tx.Payments)
                    .ThenInclude(p=> p.PatientPayment)
            .Include(tx=> tx.Diagnosis)
                .ThenInclude(tx=> tx.Payments)
                    .ThenInclude(p=> p.WoundedPayment)
            .Include(tx=> tx.Diagnosis)
                .ThenInclude(tx=> tx.Payments)
                    .ThenInclude(p=> p.DisabledPayment)
            .FirstOrDefaultAsync(tc => tc.Id == command.TherapyCardId, ct);

        if (therapyCard is null)
        {
            return Error.NotFound(
                $"كرت الاصلاح بالمعرف {command.TherapyCardId} غير موجود.");
        }


        var printedDocument =
            await _context.PrintedDocuments.GetOrCreateAsync(
                nameof(DocumentTypes.TherapyCard),
                therapyCard.Id,
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
            var pdfBytes = _therapyCardPdfGenerator.Generate(therapyCard, printContext);

            return new PdfDto
            {
                Content = pdfBytes,
                FileName = $"TherapyCard_{therapyCard.Id}.pdf",
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate PDF for TherapyCardId: {TherapyCardId}", command.TherapyCardId);
            return Error.Failure("An error occurred while generating the therapy card PDF.");
        }
    }
}