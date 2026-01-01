namespace AlatrafClinic.Application.Features.Tickets.Dtos;

public class TicketForServiceDto
{
    public int TicketId { get; set; }
    public string TicketStatus { get; set; } = string.Empty;
    public int PatientId { get; set; }
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string? PatientName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
    public string? PatientType { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public int Age  { get; set; }
}