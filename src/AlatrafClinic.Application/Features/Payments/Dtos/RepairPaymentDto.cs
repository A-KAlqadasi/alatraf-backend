using AlatrafClinic.Application.Features.RepairCards.Dtos;

namespace AlatrafClinic.Application.Features.Payments.Dtos;

public class RepairPaymentDto
{
    public int PaymentId { get; set; }
    public string PatientName { get; set; } = null!;
    public int Age { get; set; }
    public string Gender { get; set; } = null!;
    public int PatientId { get; set; }
    public List<DiagnosisIndustrialPartDto> DiagnosisIndustrialParts { get; set; } =new();
    public bool IsCompleted { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? PaidAmount { get; set; }
    public decimal? Discount { get; set; }
    public string? AccountKind {get; set; }
    public DateTime? PaymentDate { get; set; }
}