using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Departments.Sections.Rooms;

public static class RoomErrors
{
    public static readonly Error InvalidName =
        Error.Validation("Room.InvalidName", "اسم الغرفة غير صالح");
    public static readonly Error InvalidSection =
        Error.Validation("Room.InvalidSection", "القسم غير صالح");
    public static readonly Error DuplicateRoomName = Error.Conflict(
        code: "Room.DuplicateRoomName",
        description: "يوجد غرفة بنفس الاسم في هذا القسم");
    public static readonly Error NotFound = Error.NotFound(
        code: "Room.NotFound",
        description: "الغرفة المحددة غير موجودة");
}