using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Payments.Commands.PayPayments;

public class PayPaymentCommandHandler : IRequestHandler<PayPaymentCommand, Result<Updated>>
{
    private readonly ILogger<PayPaymentCommandHandler> _logger;
    private readonly IAppDbContext _context;
    private readonly IEnumerable<IPaymentTypeHandler> _handlers;

    public PayPaymentCommandHandler(ILogger<PayPaymentCommandHandler> logger, IAppDbContext context, IEnumerable<IPaymentTypeHandler> handlers)
    {
        _logger = logger;
        _context = context;
        _handlers = handlers;
    }

    public async Task<Result<Updated>> Handle(PayPaymentCommand command, CancellationToken ct)
    {
        var payment = await _context.Payments
        .Include(p=> p.PatientPayment)
        .Include(p=> p.DisabledPayment)
        .Include(p=> p.WoundedPayment)
        .FirstOrDefaultAsync(p=> p.Id == command.PaymentId, ct);
        if (payment == null)
        {
            _logger.LogWarning("Payment with Id {PaymentId} not found", command.PaymentId);
            return PaymentErrors.PaymentNotFound;
        }


        var handler = _handlers.FirstOrDefault(h => h.Kind == command.AccountKind);
        if (handler == null)
        {
            _logger.LogWarning("No handler found for AccountKind {AccountKind}", command.AccountKind);
            return PaymentErrors.InvalidAccountKind;
        } 

        if (payment.AccountKind != command.AccountKind)
            payment.ClearPaymentType();

        object typeDto = command.AccountKind switch
        {
            AccountKind.Patient => command.PatientPayment!,
            AccountKind.Disabled => command.DisabledPayment!,
            AccountKind.Wounded => command.WoundedPayment!,
            AccountKind.Free => null!,
            _ => null!
        };

        var result = await handler.HandleCreateAsync(payment, typeDto, _context, ct);
        if (result.IsError)
        {
            _logger.LogWarning("Error handling payment type for Payment Id {PaymentId}: {Errors}", command.PaymentId, result.Errors);
            return result.Errors;
        }

        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("Payment with Id {PaymentId} paid successfully", command.PaymentId);
        return Result.Updated;
    }
}
