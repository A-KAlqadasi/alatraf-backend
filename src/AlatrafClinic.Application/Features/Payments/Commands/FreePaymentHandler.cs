using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments;

namespace AlatrafClinic.Application.Features.Payments.Commands;

public class FreePaymentHandler : IPaymentTypeHandler
{
    public AccountKind Kind => AccountKind.Free;

    public Task<Result<Updated>> HandleCreateAsync(Payment payment, object typeDto, IAppDbContext context, CancellationToken ct)
    {
        payment.Pay(null, null);
        payment.ClearPaymentType();
        payment.MarkAccountKind(AccountKind.Free);

        return Task.FromResult<Result<Updated>>(Result.Updated);
    }

    public Task<Result<Updated>> HandleUpdateAsync(Payment payment, object typeDto, IAppDbContext context, CancellationToken ct)
    {
        payment.Pay(null, null);
        payment.ClearPaymentType();
        payment.MarkAccountKind(AccountKind.Free);
        
        return Task.FromResult<Result<Updated>>(Result.Updated);
    }
}