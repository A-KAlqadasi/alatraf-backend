using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments;

using MediatR;

namespace AlatrafClinic.Application.Features.Payments.Commands.PayPayments;

public sealed record PayFreePaymentCommand(int PaymentId) : IRequest<Result<Updated>>;


public sealed class PayFreePaymentCommandHandler
    : IRequestHandler<PayFreePaymentCommand, Result<Updated>>
{
    private readonly PaymentProcessor _processor;

    public PayFreePaymentCommandHandler(PaymentProcessor processor)
    {
        _processor = processor;
    }

    public async Task<Result<Updated>> Handle(PayFreePaymentCommand command, CancellationToken ct)
    {
        var load = await _processor.LoadForPayOnceAsync(command.PaymentId, ct);
        if (load.IsError) return load.Errors;

        var payment = load.Value;

        payment.ClearPaymentType();
        payment.MarkAccountKind(AccountKind.Free);

        
        var payResult = payment.Pay(null, null);
        if (payResult.IsError) return payResult.Errors;

        return await _processor.SaveAsync(payment.Id, ct);
    }
}
