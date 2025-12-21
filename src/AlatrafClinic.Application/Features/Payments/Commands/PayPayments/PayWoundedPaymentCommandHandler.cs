using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments.WoundedPayments;
using AlatrafClinic.Domain.WoundedCards;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Payments.Commands.PayPayments;

public sealed record PayWoundedPaymentCommand(
    int PaymentId,
    int WoundedCardId,
    string? ReportNumber,
    string? Notes
) : IRequest<Result<Updated>>;


public sealed class PayWoundedPaymentCommandHandler
    : IRequestHandler<PayWoundedPaymentCommand, Result<Updated>>
{
    private readonly IAppDbContext _context;
    private readonly PaymentProcessor _processor;

    // You said: if total > 30000 report is required
    private const decimal MinTotalForReportNumber = 30000m;

    public PayWoundedPaymentCommandHandler(IAppDbContext context, PaymentProcessor processor)
    {
        _context = context;
        _processor = processor;
    }

    public async Task<Result<Updated>> Handle(PayWoundedPaymentCommand command, CancellationToken ct)
    {
        var load = await _processor.LoadForPayOnceAsync(command.PaymentId, ct);
        if (load.IsError) return load.Errors;

        var payment = load.Value;

        var exists = await _context.WoundedCards.AnyAsync(w => w.Id == command.WoundedCardId, ct);
        if (!exists) return WoundedCardErrors.WoundedCardNotFound;

        // Wounded => Paid/Discount null, completed.
        var payResult = payment.Pay(null, null);
        if (payResult.IsError) return payResult.Errors;

        var woundedPaymentResult = WoundedPayment.Create(
            paymentId: payment.Id,
            total: payment.TotalAmount,
            minPriceForReportNumber: MinTotalForReportNumber,
            woundedCardId: command.WoundedCardId,
            reportNumber: command.ReportNumber,
            notes: command.Notes);

        if (woundedPaymentResult.IsError) return woundedPaymentResult.Errors;

        var assignResult = payment.AssignWoundedPayment(woundedPaymentResult.Value);
        if (assignResult.IsError) return assignResult.Errors;

        return await _processor.SaveAsync(payment.Id, ct);
    }
}