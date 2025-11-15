using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Organization.DoctorSectionRooms;

public static class DoctorSectionRoomErrors
{
    public static readonly Error DoctorIdRequired =
        Error.Validation("DoctorSectionRoom.DoctorIdRequired", "Doctor Id is required.");

    public static readonly Error SectionIdRequired =
        Error.Validation("DoctorSectionRoom.SectionIdRequired", "Section Id is required.");

    public static readonly Error RoomIdRequired =
        Error.Validation("DoctorSectionRoom.RoomIdRequired", "Room Id is required.");

    public static readonly Error AssignmentAlreadyEnded =
        Error.Validation("DoctorSectionRoom.AssignmentAlreadyEnded", "The assignment has already ended.");
    public static readonly Error DoctorSectionRoomNotFound =
        Error.NotFound("DoctorSectionRoom.NotFound", "Doctor section room not found.");
}