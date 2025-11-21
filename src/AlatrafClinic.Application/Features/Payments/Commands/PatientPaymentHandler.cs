using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.Payments.PatientPayments;

namespace AlatrafClinic.Application.Features.Payments.Commands;

// PatientPaymentHandler.cs
public class PatientPaymentHandler : IPaymentTypeHandler
{
    public AccountKind Kind => AccountKind.Patient;

    public async Task<Result<Updated>> HandleCreateAsync(Payment payment, object typeDto, IUnitOfWork uow, CancellationToken ct)
    {
        var dto = typeDto as PatientPaymentDto ?? throw new InvalidOperationException();

        payment.Pay(payment.PaidAmount, payment.Discount);
        
        var patientPaymentResult = PatientPayment.Create(dto.VoucherNumber, payment.Id, dto.Notes);
        if (patientPaymentResult.IsError) return patientPaymentResult.Errors;

        return payment.AssignPatientPayment(patientPaymentResult.Value);
    }

    public async Task<Result<Updated>> HandleUpdateAsync(Payment payment, object typeDto, IUnitOfWork uow, CancellationToken ct)
    {
        var dto = typeDto as PatientPaymentDto ?? throw new InvalidOperationException();

        var payResult = payment.Pay(payment.PaidAmount, payment.Discount);
        if (payResult.IsError) return payResult.Errors;

        if (payment.PatientPayment == null)
        {
            var createResult = PatientPayment.Create(dto.VoucherNumber, payment.Id, dto.Notes);
            if (createResult.IsError) return createResult.Errors;
            return payment.AssignPatientPayment(createResult.Value);
        }

        return payment.PatientPayment.Update(dto.VoucherNumber, dto.Notes);
    }
}
