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

    public static readonly Error AssignDateRequired =
        Error.Validation("DoctorSectionRoom.AssignDateRequired", "Assign Date is required.");

    public static readonly Error AssignDateInvalid =
        Error.Validation("DoctorSectionRoom.AssignDateInvalid", "Assign Date is invalid.");

    public static readonly Error IsActiveRequired =
        Error.Validation("DoctorSectionRoom.IsActiveRequired", "Is Active is required.");
    public static readonly Error AssignmentAlreadyEnded =
        Error.Validation("DoctorSectionRoom.AssignmentAlreadyEnded", "The assignment has already ended.");
}