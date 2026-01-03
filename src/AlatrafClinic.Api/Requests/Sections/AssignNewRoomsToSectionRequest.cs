namespace AlatrafClinic.Api.Requests.Sections;

public class AssignNewRoomsToSectionRequest
{
    public List<string> RoomNames { get; set; } = new();
}