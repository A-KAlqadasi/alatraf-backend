using System.Collections.Immutable;

using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Domain.Payments;

namespace AlatrafClinic.Application.Features.Payments.Mappers;

public static class PaymentMapper
{
    
    public static PaymentDto ToDto(this Payment p)
    {
        ArgumentNullException.ThrowIfNull(p);
        return new PaymentDto
        {
            PaymentId = p.Id,
            TicketId = p.TicketId,
            PaymentReference = p.PaymentReference,
            Diagnosis = p.Diagnosis != null ? p.Diagnosis.ToDto() : new DiagnosisDto(),
            AccountKind = p.AccountKind,
            IsCompleted = p.IsCompleted,
            TotalAmount = p.TotalAmount,
            PaidAmount = p.PaidAmount,
            Discount = p.Discount,
            PatientPayment = p.PatientPayment != null
                    ? new PatientPaymentDto
                    {
                        VoucherNumber = p.PatientPayment.VoucherNumber,
                        Notes = p.PatientPayment.Notes
                    }
                    : null,
            DisabledPayment = p.DisabledPayment != null
                    ? new DisabledPaymentDto
                    {
                        DisabledCardId = p.DisabledPayment.DisabledCardId,
                        Notes = p.DisabledPayment.Notes
                    }
                    : null,
            WoundedPayment = p.WoundedPayment != null
                    ? new WoundedPaymentDto
                    {
                        WoundedCardId = p.WoundedPayment.WoundedCardId,
                        ReportNumber = p.WoundedPayment.ReportNumber,
                        Notes = p.WoundedPayment.Notes
                    }
                    : null
        };
    }
    public static List<PaymentDto> ToDtos(this IEnumerable<Payment> payments)
    {
        return payments.Select(p => p.ToDto()).ToList();
    }
    
}