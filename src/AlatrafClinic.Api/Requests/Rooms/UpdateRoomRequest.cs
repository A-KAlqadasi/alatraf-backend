using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.Rooms;

public sealed class UpdateRoomRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string NewName { get; init; } = string.Empty;
}
