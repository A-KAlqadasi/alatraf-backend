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
            Residual = payment.Residual,
            Notes = payment.DisabledPayment?.Notes,
            CardNumber = payment.DisabledPayment?.DisabledCard?.CardNumber,
            PaymentDate = payment.CreatedAtUtc.DateTime
        };
    }
    public static List<DisabledPaymentDto> ToDisabledPaymentDtos(this IEnumerable<Payment> payments)
    {
        ArgumentNullException.ThrowIfNull(payments);

        return payments.Select(p => p.ToDisabledPaymentDto()).ToList();
    }
    public static PatientPaymentDto ToPatientPaymentDto(this Payment payment)
    {
        ArgumentNullException.ThrowIfNull(payment);

        return new PatientPaymentDto
        {
            Id = payment.Id,
            TotalAmount = payment.TotalAmount,
            PaidAmmount = payment.PaidAmount,
            DiscountAmount = payment.Discount,
            DiagnosisId = payment.DiagnosisId,
            Diagnosis = payment.Diagnosis.ToDto(),
            PatientName = payment.Diagnosis?.Patient?.Person?.FullName ?? string.Empty,
            AccountId = payment.AccountId,
            AccountName = payment.Account?.AccountName,
            PaymentType = payment.Type,
            IsCompleted = payment.IsCompleted,
            Residual = payment.Residual,
            VoucherNumber = payment.PatientPayment?.VoucherNumber,
            Notes = payment.PatientPayment?.Notes,
            PaymentDate = payment.CreatedAtUtc.DateTime
        };
    }
    public static List<PatientPaymentDto> ToPatientPaymentDtos(this IEnumerable<Payment> payments)
    {
        ArgumentNullException.ThrowIfNull(payments);

        return payments.Select(p => p.ToPatientPaymentDto()).ToList();
    }
    public static WoundedPaymentDto ToWoundedPaymentDto(this Payment payment)
    {
        ArgumentNullException.ThrowIfNull(payment);

        return new WoundedPaymentDto
        {
            Id = payment.Id,
            CardNumber = payment.WoundedPayment?.WoundedCard?.CardNumber,
            ReportNumber = payment.WoundedPayment?.ReportNumber,
            TotalAmount = payment.TotalAmount,
            DiagnosisId = payment.DiagnosisId,
            Diagnosis = payment.Diagnosis.ToDto(),
            PatientName = payment.Diagnosis?.Patient?.Person?.FullName ?? string.Empty,
            AccountId = payment.AccountId,
            AccountName = payment.Account?.AccountName,
            PaymentType = payment.Type,
            IsCompleted = payment.IsCompleted,
            Residual = payment.Residual,
            Notes = payment.WoundedPayment?.Notes,
            PaymentDate = payment.CreatedAtUtc.DateTime
        };
    }
    
    public static List<WoundedPaymentDto> ToWoundedPaymentDtos(this IEnumerable<Payment> payments)
    {
        ArgumentNullException.ThrowIfNull(payments);

        return payments.Select(p => p.ToWoundedPaymentDto()).ToList();
    }
}