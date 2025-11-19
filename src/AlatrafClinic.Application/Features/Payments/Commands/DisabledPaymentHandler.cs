using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Domain.Accounts;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Cards.DisabledCards;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.Payments.DisabledPayments;

namespace AlatrafClinic.Application.Features.Payments.Commands;

// DisabledPaymentHandler.cs
public class DisabledPaymentHandler : IPaymentTypeHandler
{
    public AccountKind Kind => AccountKind.Disabled;

    public async Task<Result<Updated>> HandleCreateAsync(Payment payment, object typeDto, IUnitOfWork uow, CancellationToken ct)
    {
        var dto = typeDto as DisabledPaymentDto ?? throw new InvalidOperationException();
        var exists = await uow.Patients.IsDisabledCardExists(dto.DisabledCardId, ct);
        if (!exists) return DisabledCardErrors.DisabledCardNotFound;

        payment.Pay(null, null, dto.AccountId);

        var result = DisabledPayment.Create(dto.DisabledCardId, payment.Id, dto.Notes);
        if (result.IsError) return result.Errors;

        return payment.AssignDisabledPayment(result.Value);
    }

    public async Task<Result<Updated>> HandleUpdateAsync(Payment payment, object typeDto, IUnitOfWork uow, CancellationToken ct)
    {
        var dto = typeDto as DisabledPaymentDto ?? throw new InvalidOperationException();
        var exists = await uow.Patients.IsDisabledCardExists(dto.DisabledCardId, ct);
        if (!exists) return DisabledCardErrors.DisabledCardNotFound;

        payment.Pay(null, null, dto.AccountId);

        if (payment.DisabledPayment == null)
        {
            var createResult = DisabledPayment.Create(dto.DisabledCardId, payment.Id, dto.Notes);
            if (createResult.IsError) return createResult.Errors;
            return payment.AssignDisabledPayment(createResult.Value);
        }

        return payment.DisabledPayment.Update(dto.DisabledCardId, dto.Notes);
    }
}
