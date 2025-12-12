using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.RepairCards;

public sealed class CreateDeliveryTimeRequest
{
    [Required]
    [DataType(DataType.Date)]
    public DateOnly DeliveryDate { get; set; }

    [MaxLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
    public string? Notes { get; set; }
}