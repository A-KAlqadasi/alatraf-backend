using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Printing;
using AlatrafClinic.Application.Common.Printing.Dtos;
using AlatrafClinic.Application.Common.Printing.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.PrintRepairCard;

public class PrintRepairCardCommandHandler
    : IRequestHandler<PrintRepairCardCommand, Result<PdfDto>>
{
    private readonly IPdfGenerator<RepairCard> _repairCardPdfGenerator;
    private readonly IAppDbContext _context;
    private readonly ILogger<PrintRepairCardCommandHandler> _logger;
    public PrintRepairCardCommandHandler(
        IPdfGenerator<RepairCard> repairCardPdfGenerator,
        IAppDbContext context, ILogger<PrintRepairCardCommandHandler> logger)
    {
        _repairCardPdfGenerator = repairCardPdfGenerator;
        _context = context;
        _logger = logger;
    }

    public async Task<Result<PdfDto>> Handle(
        PrintRepairCardCommand command,
        CancellationToken ct)
    {
        var repairCard = await _context.RepairCards
        .Include(r => r.Diagnosis)
        .ThenInclude(d => d.Patient)
            .ThenInclude(p => p.Person)
        .Include(r=> r.DiagnosisIndustrialParts)
            .ThenInclude(i=> i.IndustrialPartUnit)
                .ThenInclude(u=> u.IndustrialPart)
        .Include(r=> r.DiagnosisIndustrialParts)
            .ThenInclude(i=> i.IndustrialPartUnit)
                .ThenInclude(u=> u.Unit)
        .Include(r => r.DiagnosisIndustrialParts)
            .ThenInclude(i => i.DoctorSectionRoom!)
                .ThenInclude(d => d.Section)
        .Include(r => r.DiagnosisIndustrialParts)
            .ThenInclude(i => i.DoctorSectionRoom!)
                .ThenInclude(d => d.Doctor)
                    .ThenInclude(doc => doc.Person)
        .Include(r => r.Diagnosis)
            .ThenInclude(d=> d.Payments)
        .FirstOrDefaultAsync(r=> r.Id == command.RepairCardId, ct);

        if (repairCard is null)
        {
            return Error.NotFound(
                $"كرت الاصلاح بالمعرف {command.RepairCardId} غير موجود.");
        }

        if (repairCard.Status is Domain.RepairCards.Enums.RepairCardStatus.New )
        {
            return Error.Conflict("RepairCardNotAssigned",
                "لا يمكن طباعة كرت الاصلاح قبل تعيينه لفني.");
        }

        var printedDocument =
            await _context.PrintedDocuments.GetOrCreateAsync(
                nameof(DocumentTypes.RepairCard),
                repairCard.Id,
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
            var pdfBytes = _repairCardPdfGenerator.Generate(repairCard, printContext);

            return new PdfDto
            {
                Content = pdfBytes,
                FileName = $"RepairCard_{repairCard.Id}.pdf",
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate PDF for RepairCardId: {RepairCardId}", command.RepairCardId);
            return Error.Failure("An error occurred while generating the repair card PDF.");
        }
    }
}