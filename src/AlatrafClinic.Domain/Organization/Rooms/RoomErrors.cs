using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Organization.Rooms;

public static class RoomErrors
{
    public static readonly Error InvalidNumber =
        Error.Validation("Room.InvalidNumber", "Room number is invalid.");
    public static readonly Error InvalidSection =
        Error.Validation("Room.InvalidSection", "Section is invalid.");
}