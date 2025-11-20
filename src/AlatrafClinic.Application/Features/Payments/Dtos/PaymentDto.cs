using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.Payments;

namespace AlatrafClinic.Application.Features.Payments.Dtos;

public class PaymentDto
{
    public int PaymentId { get; set; }
    public DiagnosisDto Diagnosis { get; set; } = new();
    public AccountKind AccountKind { get; set; }
    public bool IsCompleted { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? PaidAmount { get; set; }
    public decimal? Discount { get; set; }

    public PatientPaymentDto? PatientPayment { get; set; }
    public DisabledPaymentDto? DisabledPayment { get; set; }
    public WoundedPaymentDto? WoundedPayment { get; set; }
}