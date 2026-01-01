using System.ComponentModel.DataAnnotations;


namespace AlatrafClinic.Api.Requests.Tickets;

public sealed class ScheduleAppointmentRequest
{
    
    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    public string? Notes { get; init; }
    public DateOnly? RequestedDate { get; init; }
}