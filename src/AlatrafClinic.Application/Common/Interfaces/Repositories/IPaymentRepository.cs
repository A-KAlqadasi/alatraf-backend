using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.Payments.DisabledPayments;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IPaymentRepository : IGenericRepository<Payment, int>
{
    Task AddDisabledPaymentAsync(DisabledPayment disabledPayment, CancellationToken ct);
    Task<DisabledPayment?> GetDisabledPaymentByPaymentIdAsync(int paymentId, CancellationToken ct);
    Task UpdateDisabledPaymentAsync(DisabledPayment disabledPayment, CancellationToken ct);
    
}