
using System.ComponentModel.DataAnnotations;

using AlatrafClinic.Domain.Tickets;

namespace AlatrafClinic.Api.Requests.Tickets;

public class UpdateTicketRequest
{
    [Required]
    [Range(1, 9, ErrorMessage = "ServiceId must be between 1 - 9.")]
    public int ServiceId { get; set; }
    
    [Required]
    public int PatientId { get; set; }
    
    public TicketStatus? Status { get; set; } = null;
}