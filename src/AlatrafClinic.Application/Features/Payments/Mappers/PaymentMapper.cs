using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Domain.Payments;

namespace AlatrafClinic.Application.Features.Payments.Mappers;

public static class PaymentMapper
{
    public static DisabledPaymentDto ToDisabledPaymentDto(this Payment payment)
    {
        ArgumentNullException.ThrowIfNull(payment);

        return new DisabledPaymentDto
        {
            Id = payment.Id,
            TotalAmount = payment.TotalAmount,
            DiagnosisId = payment.DiagnosisId,
            Diagnosis = payment.Diagnosis.ToDto(),
            PatientName = payment.Diagnosis?.Patient?.Person?.FullName ?? string.Empty,
            AccountId = payment.AccountId,
            AccountName = payment.Account?.AccountName,
            PaymentType = payment.Type,
            IsCompleted = payment.IsCompleted,
            Residual = payment.Residual
        };
    }
    public static List<DisabledPaymentDto> ToDisabledPaymentDtos(this IEnumerable<Payment> payments)
    {
        ArgumentNullException.ThrowIfNull(payments);

        return payments.Select(p => p.ToDisabledPaymentDto()).ToList();
    }
}