using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.Payments;

namespace AlatrafClinic.Application.Features.Payments.Dtos;

public class PatientPaymentDto
{
    public string VoucherNumber { get; set; } = string.Empty;
    public string? Notes { get; set; }
}