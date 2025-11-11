using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Organization.Rooms;

public static class RoomErrors
{
    public static readonly Error InvalidNumber =
        Error.Validation("Room.InvalidNumber", "Room number is invalid.");
    public static readonly Error InvalidSection =
        Error.Validation("Room.InvalidSection", "Section is invalid.");
    public static readonly Error DuplicateRoomNumber = Error.Validation(
   code: "Room.DuplicateRoomNumber",
   description: "A room with the same number already exists in this section.");

    public static readonly Error AlreadyDeleted =
        Error.Conflict("Room.AlreadyDeleted", "This room is already deleted.");

    public static readonly Error NotDeleted =
        Error.Conflict("Room.NotDeleted", "This room is not deleted.");

}