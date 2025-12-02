

using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.Payments.DisabledPayments;
using AlatrafClinic.Domain.Payments.PatientPayments;
using AlatrafClinic.Domain.Payments.WoundedPayments;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class PaymentRepository : GenericRepository<Payment, int>, IPaymentRepository
{
    public PaymentRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
        
    }

    public async Task<bool> IsVoucherNumberExistsAsync(string voucherNumber, CancellationToken ct)
    {
        return await dbContext.PatientPayments
            .AnyAsync(p => p.VoucherNumber == voucherNumber, ct);
    }
}