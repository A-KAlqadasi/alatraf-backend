using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.Payments;

namespace AlatrafClinic.Application.Features.Payments.Dtos;

public class WoundedPaymentDto
{
    public int WoundedCardId { get; set; }
    public string? ReportNumber { get; set; }
    public string? Notes { get; set; }
}