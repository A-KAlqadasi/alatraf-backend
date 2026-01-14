
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Payments.Commands.PayPayments;

public sealed class PaymentProcessor
{
    private readonly IAppDbContext _context;
    private readonly ILogger<PaymentProcessor> _logger;

    public PaymentProcessor(IAppDbContext context, ILogger<PaymentProcessor> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<Payment>> LoadForPayOnceAsync(int paymentId, CancellationToken ct)
    {
        var payment = await _context.Payments
            .Include(p=> p.Ticket)
            .Include(p => p.PatientPayment)
            .Include(p => p.DisabledPayment)
            .Include(p => p.WoundedPayment)
            .FirstOrDefaultAsync(p => p.Id == paymentId, ct);

        if (payment is null)
        {
            _logger.LogWarning("Payment with Id {PaymentId} not found", paymentId);
            return PaymentErrors.PaymentNotFound;
        }

        // One-time pay rule (future-proof: later you can make this conditional)
        if (payment.IsCompleted)
        {
            _logger.LogWarning("Payment with Id {PaymentId} is already completed", paymentId);

            return PaymentErrors.PaymentAlreadyCompleted;
        }

        return payment;
    }

    public async Task<Result<Updated>> SaveAsync(int paymentId, CancellationToken ct)
    {
        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("Payment {PaymentId} paid successfully", paymentId);
        return Result.Updated;
    }
}

