using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.DisabledCards;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.Payments.DisabledPayments;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Payments.Commands;

public class DisabledPaymentHandler : IPaymentTypeHandler
{
    public AccountKind Kind => AccountKind.Disabled;

    public async Task<Result<Updated>> HandleCreateAsync(Payment payment, object typeDto, IAppDbContext context, CancellationToken ct)
    {
        var dto = typeDto as DisabledPaymentDto ?? throw new InvalidOperationException();
        var exists = await context.DisabledCards.AnyAsync(d=> d.Id == dto.DisabledCardId, ct);
        if (!exists) return DisabledCardErrors.DisabledCardNotFound;

        payment.Pay(null, null);

        var result = DisabledPayment.Create(dto.DisabledCardId, payment.Id, dto.Notes);
        if (result.IsError) return result.Errors;

        return payment.AssignDisabledPayment(result.Value);
    }

    public async Task<Result<Updated>> HandleUpdateAsync(Payment payment, object typeDto, IAppDbContext context, CancellationToken ct)
    {
        var dto = typeDto as DisabledPaymentDto ?? throw new InvalidOperationException();
        var exists = await context.DisabledCards.AnyAsync(d=> d.Id == dto.DisabledCardId, ct);
        if (!exists) return DisabledCardErrors.DisabledCardNotFound;

        payment.Pay(null, null);

        if (payment.DisabledPayment == null)
        {
            var createResult = DisabledPayment.Create(dto.DisabledCardId, payment.Id, dto.Notes);
            if (createResult.IsError) return createResult.Errors;
            return payment.AssignDisabledPayment(createResult.Value);
        }

        return payment.DisabledPayment.Update(dto.DisabledCardId, dto.Notes);
    }
}
