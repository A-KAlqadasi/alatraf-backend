using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments;

namespace AlatrafClinic.Application.Features.Payments.Commands;

public interface IPaymentTypeHandler
{
    AccountKind Kind { get; }
    Task<Result<Updated>> HandleCreateAsync(Payment payment, object typeDto, IUnitOfWork uow, CancellationToken ct);
    Task<Result<Updated>> HandleUpdateAsync(Payment payment, object typeDto, IUnitOfWork uow, CancellationToken ct);
}
