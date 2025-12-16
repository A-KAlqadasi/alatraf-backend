namespace AlatrafClinic.Api.Requests.Doctors;

public sealed class TechnicianFilterRequest
{
    public int? SectionId { get; init; }

    public string? SearchTerm { get; init; }
}
