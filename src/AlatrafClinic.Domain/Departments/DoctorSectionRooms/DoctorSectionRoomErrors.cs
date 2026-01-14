using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Departments.DoctorSectionRooms;

public static class DoctorSectionRoomErrors
{
    public static readonly Error DoctorIdRequired =
        Error.Validation("DoctorSectionRoom.DoctorIdRequired", "رقم الطبيب مطلوب");

    public static readonly Error SectionIdRequired =
        Error.Validation("DoctorSectionRoom.SectionIdRequired", "رقم القسم مطلوب");

    public static readonly Error RoomIdRequired =
        Error.Validation("DoctorSectionRoom.RoomIdRequired", "رقم الغرفة مطلوب");

    public static readonly Error AssignmentAlreadyEnded =
        Error.Conflict("DoctorSectionRoom.AssignmentAlreadyEnded", "انتهى تعيين الدكتور في هذه الغرفة");
    public static readonly Error DoctorSectionRoomNotFound =
        Error.NotFound("DoctorSectionRoom.NotFound", "معرف غرفة قسم الطبيب غير موجود.");
    public static readonly Error DoctorIsNotActive = Error.Conflict("DoctorSectionRoom.DoctorIsNotActive", "الطبيب غير نشط في هذه الغرفة");
}