using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.Payments;

namespace AlatrafClinic.Application.Features.Payments.Dtos;

public record DisabledPaymentDto
{
    public int DisabledCardId { get; set; }
    public string? Notes { get; set; } = null;
}