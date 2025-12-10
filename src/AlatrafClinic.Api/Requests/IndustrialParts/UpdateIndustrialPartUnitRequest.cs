using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.IndustrialParts;

public class UpdateIndustrialPartUnitRequest 
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "UnitId must be positive.")]
    public int UnitId { get; set; } = default!;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    public decimal Price { get; set; }
}