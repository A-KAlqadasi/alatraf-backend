using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.Payments;

namespace AlatrafClinic.Application.Features.Payments.Dtos;

public class DisabledPaymentDto
{
    public int Id { get; set; }
    public decimal TotalAmount { get; set; }
    public int DiagnosisId { get; set; }
    public DiagnosisDto Diagnosis { get; set; } = default!;
    public string PatientName { get; set; } = default!;
    public int? AccountId { get; set; }
    public string? AccountName { get; set; } 
    public PaymentType PaymentType { get; set; } = default!;
    public bool IsCompleted { get; set; }
    public decimal Residual { get; set; }
}