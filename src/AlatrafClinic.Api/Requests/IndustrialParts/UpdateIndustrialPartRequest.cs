using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.IndustrialParts;

public class UpdateIndustrialPartRequest
{
    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    
    [Required]
    [MinLength(1, ErrorMessage = "At least one unit is required.")]
    public List<CreateIndustrialPartUnitRequest> Units { get; set; } = new();
}
