using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.Rooms;

public sealed class CreateRoomRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "SectionId must be greater than 0.")]
    public int SectionId { get; init; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;
}