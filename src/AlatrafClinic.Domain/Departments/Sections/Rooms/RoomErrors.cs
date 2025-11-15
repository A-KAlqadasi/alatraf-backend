using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Organization.Rooms;

public static class RoomErrors
{
    public static readonly Error InvalidName =
        Error.Validation("Room.InvalidName", "Room name is invalid.");
    public static readonly Error InvalidSection =
        Error.Validation("Room.InvalidSection", "Section is invalid.");
    public static readonly Error DuplicateRoomName = Error.Validation(
        code: "Room.DuplicateRoomName",
        description: "A room with the same name already exists in this section.");

    public static readonly Error AlreadyDeleted =
        Error.Conflict("Room.AlreadyDeleted", "This room is already deleted.");

    public static readonly Error NotDeleted =
        Error.Conflict("Room.NotDeleted", "This room is not deleted.");

}