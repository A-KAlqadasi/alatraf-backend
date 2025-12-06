using AlatrafClinic.Domain.Services.Enums;

namespace AlatrafClinic.Api.Requests.Tickets;

public class TicketFilterRequest
{
    public string? SearchTerm { get; set; }
    public string SortBy { get; set; } = "createdAt";
    public string SortDirection { get; set; } = "desc";
    public int? PatientId { get; set; }
    public int? ServiceId { get; set; }
    public int? DepartmentId { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public TicketStatus? Status { get; set; }
}