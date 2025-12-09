using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.Injuries;

public class CreateInjuryRequest
{
    [Required(ErrorMessage = "Injury name is required.")]
    [MaxLength(100, ErrorMessage = "Injury name must not exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;
}
