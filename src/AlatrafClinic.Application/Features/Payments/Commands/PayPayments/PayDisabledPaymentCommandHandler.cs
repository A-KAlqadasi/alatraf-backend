using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.DisabledCards;
using AlatrafClinic.Domain.Payments.DisabledPayments;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Payments.Commands.PayPayments;

public sealed record PayDisabledPaymentCommand(
    int PaymentId,
    int DisabledCardId,
    string? Notes
) : IRequest<Result<Updated>>;

public sealed class PayDisabledPaymentCommandHandler
    : IRequestHandler<PayDisabledPaymentCommand, Result<Updated>>
{
    private readonly IAppDbContext _context;
    private readonly PaymentProcessor _processor;

    public PayDisabledPaymentCommandHandler(IAppDbContext context, PaymentProcessor processor)
    {
        _context = context;
        _processor = processor;
    }

    public async Task<Result<Updated>> Handle(PayDisabledPaymentCommand command, CancellationToken ct)
    {
        var load = await _processor.LoadForPayOnceAsync(command.PaymentId, ct);
        if (load.IsError) return load.Errors;

        var payment = load.Value;

        var exists = await _context.DisabledCards.AnyAsync(d => d.Id == command.DisabledCardId, ct);
        if (!exists) return DisabledCardErrors.DisabledCardNotFound;

        // Disabled => Paid/Discount null, completed.
        var payResult = payment.Pay(null, null);
        if (payResult.IsError) return payResult.Errors;

        var disabledPaymentResult = DisabledPayment.Create(command.DisabledCardId, payment.Id, command.Notes);
        if (disabledPaymentResult.IsError) return disabledPaymentResult.Errors;

        var assignResult = payment.AssignDisabledPayment(disabledPaymentResult.Value);
        if (assignResult.IsError) return assignResult.Errors;

        return await _processor.SaveAsync(payment.Id, ct);
    }
}
