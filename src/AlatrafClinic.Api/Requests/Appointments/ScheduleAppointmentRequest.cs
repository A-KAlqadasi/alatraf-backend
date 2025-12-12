using System.ComponentModel.DataAnnotations;


namespace AlatrafClinic.Api.Requests.Appointments;

public sealed class ScheduleAppointmentRequest
{
    [Required(ErrorMessage = "TicketId is required")]
    [Range(1, int.MaxValue)]
    public int TicketId { get; init; }
    
    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    public string? Notes { get; init; }
    public DateOnly? RequestedDate { get; init; }
}