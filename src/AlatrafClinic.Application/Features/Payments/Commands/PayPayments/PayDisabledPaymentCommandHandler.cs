using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.DisabledCards;
using AlatrafClinic.Domain.Payments.DisabledPayments;

using MediatR;


namespace AlatrafClinic.Application.Features.Payments.Commands.PayPayments;

public sealed record PayDisabledPaymentCommand(
    int PaymentId,
    string CardNumber,
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

        var disabledCard = await _context.DisabledCards.FindAsync(new object[] { command.CardNumber.Trim() }, ct);

        if (disabledCard is null) return DisabledCardErrors.DisabledCardNotFound;
        
        if(payment.Ticket.PatientId != disabledCard.PatientId)
        {
            return DisabledCardErrors.DisabledCardDesnotBelongToPatient;
        }

        // Disabled => Paid/Discount null, completed.
        var payResult = payment.Pay(null, null);
        if (payResult.IsError) return payResult.Errors;

        var disabledPaymentResult = DisabledPayment.Create(disabledCard.Id, payment.Id, command.Notes);
        if (disabledPaymentResult.IsError) return disabledPaymentResult.Errors;

        var assignResult = payment.AssignDisabledPayment(disabledPaymentResult.Value);
        if (assignResult.IsError) return assignResult.Errors;

        return await _processor.SaveAsync(payment.Id, ct);
    }
}
