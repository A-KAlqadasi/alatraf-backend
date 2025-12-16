namespace AlatrafClinic.Api.Requests.Doctors;

public sealed class TherapistFilterRequest
{
    public int? SectionId { get; init; }

    public int? RoomId { get; init; }

    public string? SearchTerm { get; init; }
}