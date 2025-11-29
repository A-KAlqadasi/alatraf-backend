using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.Payments.DisabledPayments;
using AlatrafClinic.Domain.Payments.PatientPayments;
using AlatrafClinic.Domain.Payments.WoundedPayments;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IPaymentRepository : IGenericRepository<Payment, int>
{
    Task<bool> IsVoucherNumberExistsAsync(string voucherNumber, CancellationToken ct);
}