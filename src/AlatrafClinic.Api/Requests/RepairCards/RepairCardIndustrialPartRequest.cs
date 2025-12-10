using System.ComponentModel.DataAnnotations;


namespace AlatrafClinic.Api.Requests.RepairCards;

public sealed class RepairCardIndustrialPartRequest
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "IndustrialPartId must be positive.")]
    public int IndustrialPartId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "UnitId must be positive.")]
    public int UnitId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }
    
}
