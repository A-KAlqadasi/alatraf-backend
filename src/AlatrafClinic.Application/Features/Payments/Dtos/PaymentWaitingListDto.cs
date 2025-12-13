namespace AlatrafClinic.Application.Features.Payments.Dtos;

public class PaymentWaitingListDto
{
    public int PaymentId {get; set; }
    public int CardId { get; set; }
    public string PatientName {get; set; } = null!;
    public string? Gender {get; set; }
    public int Age { get; set; }
    public string? Phone { get; set; }
    public string PaymentReference { get; set; } = null!;
    
}