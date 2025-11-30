using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.Payments.WoundedPayments;
using AlatrafClinic.Domain.WoundedCards;

namespace AlatrafClinic.Application.Features.Payments.Commands;

// WoundedPaymentHandler.cs
public class WoundedPaymentHandler : IPaymentTypeHandler
{
    public AccountKind Kind => AccountKind.Wounded;

    public async Task<Result<Updated>> HandleCreateAsync(Payment payment, object typeDto, IUnitOfWork uow, CancellationToken ct)
    {
        var dto = typeDto as WoundedPaymentDto ?? throw new InvalidOperationException();
        var exists = await uow.WoundedCards.IsExistAsync(dto.WoundedCardId, ct);
        if (!exists) return WoundedCardErrors.WoundedCardNotFound;

        payment.Pay(null, null);

        var minPrice = 0m; // configurable
        var result = WoundedPayment.Create(payment.Id, payment.TotalAmount, minPrice, dto.WoundedCardId, dto.ReportNumber, dto.Notes);
        if (result.IsError) return result.Errors;

        return payment.AssignWoundedPayment(result.Value);
    }

    public async Task<Result<Updated>> HandleUpdateAsync(Payment payment, object typeDto, IUnitOfWork uow, CancellationToken ct)
    {
        var dto = typeDto as WoundedPaymentDto ?? throw new InvalidOperationException();
        var exists = await uow.WoundedCards.IsExistAsync(dto.WoundedCardId, ct);
        if (!exists) return WoundedCardErrors.WoundedCardNotFound;

        payment.Pay(null, null);

        var minPrice = 0m; // configurable
        if (payment.WoundedPayment == null)
        {
            var createResult = WoundedPayment.Create(payment.Id, payment.TotalAmount, minPrice, dto.WoundedCardId, dto.ReportNumber, dto.Notes);
            if (createResult.IsError) return createResult.Errors;
            return payment.AssignWoundedPayment(createResult.Value);
        }

        return payment.WoundedPayment.Update(dto.WoundedCardId, payment.TotalAmount, minPrice, dto.ReportNumber, dto.Notes);
    }
}
