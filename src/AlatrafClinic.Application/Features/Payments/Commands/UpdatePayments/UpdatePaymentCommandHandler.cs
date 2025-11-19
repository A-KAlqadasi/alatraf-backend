using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Accounts;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments;

using MediatR;

namespace AlatrafClinic.Application.Features.Payments.Commands.UpdatePayments;

public class UpdatePaymentCommandHandler : IRequestHandler<UpdatePaymentCommand, Result<Updated>>
{
    private readonly IUnitOfWork _uow;
    private readonly IEnumerable<IPaymentTypeHandler> _handlers;

    public UpdatePaymentCommandHandler(IUnitOfWork uow, IEnumerable<IPaymentTypeHandler> handlers)
    {
        _uow = uow;
        _handlers = handlers;
    }

    public async Task<Result<Updated>> Handle(UpdatePaymentCommand cmd, CancellationToken ct)
    {
        var payment = await _uow.Payments.GetByIdAsync(cmd.PaymentId, ct);
        if (payment == null) return PaymentErrors.PaymentNotFound;

        var handler = _handlers.FirstOrDefault(h => h.Kind == cmd.AccountKind);
        if (handler == null) return PaymentErrors.InvalidAccountKind;

        if (payment.AccountKind != cmd.AccountKind)
            payment.ClearPaymentType();

        object typeDto = cmd.AccountKind switch
        {
            AccountKind.Patient => cmd.PatientPayment!,
            AccountKind.Disabled => cmd.DisabledPayment!,
            AccountKind.Wounded => cmd.WoundedPayment!,
            AccountKind.Free => null!,
            _ => null!
        };

        var result = await handler.HandleUpdateAsync(payment, typeDto, _uow, ct);
        if (result.IsError) return result.Errors;

        await _uow.SaveChangesAsync(ct);
        return Result.Updated;
    }
}
