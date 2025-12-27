
using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.Tickets;

public class CreateTicketRequest
{
    public int? PatientId { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "ServiceId must be between 1 - 9.")]
    public int ServiceId { get; set; }
}