using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Printing;
using AlatrafClinic.Application.Common.Printing.Dtos;
using AlatrafClinic.Application.Common.Printing.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Payments.Commands.PrintPayment;

public class PrintPaymentCommandHandler
    : IRequestHandler<PrintPaymentCommand, Result<PdfDto>>
{
    private readonly IPdfGenerator<Payment> _paymentPdfGenerator;
    private readonly IAppDbContext _context;
    private readonly ILogger<PrintPaymentCommandHandler> _logger;
    public PrintPaymentCommandHandler(
        IPdfGenerator<Payment> paymentPdfGenerator,
        IAppDbContext context, ILogger<PrintPaymentCommandHandler> logger)
    {
        _paymentPdfGenerator = paymentPdfGenerator;
        _context = context;
        _logger = logger;
    }

    public async Task<Result<PdfDto>> Handle(
        PrintPaymentCommand command,
        CancellationToken ct)
    {
        var payment = await _context.Payments
        .AsNoTracking()
            .Include(tc => tc.Diagnosis)
                .ThenInclude(d => d.Patient)!.ThenInclude(p=> p.Person)
            .Include(tc => tc.Diagnosis)
                .ThenInclude(ir=> ir.InjuryReasons)
            .Include(tc => tc.Diagnosis)
                .ThenInclude(injs=> injs.InjurySides)
            .Include(tc => tc.Diagnosis)
                .ThenInclude(it=> it.InjuryTypes)
            .Include(tc => tc.Diagnosis)
                .ThenInclude(tc => tc.DiagnosisPrograms)
                    .ThenInclude(d=> d.MedicalProgram!)
                        .ThenInclude(d=> d.Section)
            .Include(tc => tc.Diagnosis)
                .ThenInclude(tc => tc.DiagnosisIndustrialParts)
                    .ThenInclude(d=> d.IndustrialPartUnit!)
                        .ThenInclude(d=> d.IndustrialPart)
            .Include(tc => tc.Diagnosis)
                .ThenInclude(tc => tc.DiagnosisIndustrialParts)
                    .ThenInclude(d=> d.IndustrialPartUnit!)
                        .ThenInclude(d=> d.Unit)
            .Include(tc => tc.Diagnosis)
                .ThenInclude(tc => tc.TherapyCard)
            .FirstOrDefaultAsync(tc => tc.Id == command.PaymentId, ct);

        if (payment is null)
        {
            return Error.NotFound(
                $"كرت استمارة التحويل بالمعرف {command.PaymentId} غير موجودة");
        }

        if (payment.IsCompleted)
        {
            return Error.Failure(
                $"لا يمكن طباعة استمارة التحويل بالمعرف {command.PaymentId} لانه مكتمل الدفع");
        }


        var printedDocument =
            await _context.PrintedDocuments.GetOrCreateAsync(
                nameof(DocumentTypes.Payment),
                payment.Id,
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
            var pdfBytes = _paymentPdfGenerator.Generate(payment, printContext);

            return new PdfDto
            {
                Content = pdfBytes,
                FileName = $"Payment_{payment.Id}.pdf",
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate PDF for PaymentId: {PaymentId}", command.PaymentId);
            return Error.Failure("An error occurred while generating the payment PDF.");
        }
    }
}