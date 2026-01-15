using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.Addresses;

public class UpdateAddressRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
}