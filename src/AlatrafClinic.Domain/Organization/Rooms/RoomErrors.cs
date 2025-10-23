using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Organization.Rooms;

public static class RoomErrors
{
    public static readonly Error NameRequired =
        Error.Validation("Room.NameRequired", "Room name is required.");
    public static readonly Error SectionIdRequired =
        Error.Validation("Room.SectionIdRequired", "Section Id is required.");
}