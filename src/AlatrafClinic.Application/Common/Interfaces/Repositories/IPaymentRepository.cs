using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.Payments.DisabledPayments;
using AlatrafClinic.Domain.Payments.PatientPayments;
using AlatrafClinic.Domain.Payments.WoundedPayments;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IPaymentRepository : IGenericRepository<Payment, int>
{
    Task AddDisabledPaymentAsync(DisabledPayment disabledPayment, CancellationToken ct);
    Task<DisabledPayment?> GetDisabledPaymentByPaymentIdAsync(int paymentId, CancellationToken ct);
    Task UpdateDisabledPaymentAsync(DisabledPayment disabledPayment, CancellationToken ct);

    Task AddPatientPaymentAsync(PatientPayment patientPayment, CancellationToken ct);
    Task<PatientPayment?> GetPatientPaymentByIdAsync(int id, CancellationToken ct);
    Task<bool> IsVoucherNumberExistsAsync(string voucherNumber, CancellationToken ct);
    Task UpdatePatientPaymentAsync(PatientPayment patientPayment, CancellationToken ct);

    Task AddWoundedPaymentAsync(WoundedPayment woundedPayment, CancellationToken ct);
    Task<WoundedPayment?> GetWoundedPaymentByIdAsync(int id, CancellationToken ct);
    Task UpdateWoundedPaymentAsync(WoundedPayment woundedPayment, CancellationToken ct);

    Task<IQueryable<Payment>> GetPaymentsQueryAsync(CancellationToken ct = default);
    
}