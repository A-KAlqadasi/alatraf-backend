using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Departments.Sections.Rooms;

public static class RoomErrors
{
    public static readonly Error InvalidName =
        Error.Validation("Room.InvalidName", "Room name is invalid.");
    public static readonly Error InvalidSection =
        Error.Validation("Room.InvalidSection", "Section is invalid.");
    public static readonly Error DuplicateRoomName = Error.Conflict(
        code: "Room.DuplicateRoomName",
        description: "A room with the same name already exists in this section.");
    public static readonly Error NotFound = Error.NotFound(
        code: "Room.NotFound",
        description: "The specified room was not found.");
}